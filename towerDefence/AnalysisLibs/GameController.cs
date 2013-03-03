using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHunspell;
using towerDefence;

namespace AnalysisLibs
{
    public class GameController
    {
        public delegate void SendTweetDelegate(AnalysedSentence sentence);

        private Thread listenerThread;
        private TwitterStream storedStream;
        private SpellChecker spellChecker;
        private SendTweetDelegate sendTweetDestination;

        private TimeSpan frequency;
        private DateTime lastSent;

        private static readonly object lockCookie = new object();
        
        public GameController(SendTweetDelegate sendTweetDestination)
        {
            this.sendTweetDestination = sendTweetDestination;
            string stream_url = ConfigurationManager.AppSettings["stream_url"];
            storedStream = SetupTwitterStream(stream_url);
            spellChecker = SetupSpellChecker();

            SetFreqency(new TimeSpan(0,0,0,5));
            lastSent = DateTime.MinValue;
        }

        public void SetFreqency(TimeSpan timeSpan)
        {
            lock (lockCookie)
            {
                frequency = timeSpan;
            }
            
        }

        public void SendImmediate()
        {
            lock (lockCookie)
            {
                lastSent = DateTime.MinValue;
            }
        }

        public void SetDifficulty(int difficulty)
        {
            //not used yet
        }

        public void Listen(string[] tags)
        {
            if (listenerThread != null)
            {
                storedStream.Stop();
                Thread.Sleep(2000);
                listenerThread = null;
            }
            listenerThread = new Thread(new ParameterizedThreadStart(storedStream.Stream));
            listenerThread.Start(tags);
        }

        private TwitterStream SetupTwitterStream(string url)
        {
            var language = "en";
            var user = ConfigurationManager.AppSettings["user"];
            var pass = ConfigurationManager.AppSettings["pass"];

            return new TwitterStream(url, user, pass, AnalyseTweet, language);
        }

        private void AnalyseTweet(Status status)
        {
            AnalysedSentence analysed = spellChecker.CheckSentence(status.text);
            if (analysed.IsValid && analysed.HasMisspelling)
            {
                if (sendTweetDestination != null)
                {
                    RateLimitAndSend(analysed);
                }
            }
        }

        private void RateLimitAndSend(AnalysedSentence sentence)
        {
            bool shouldSend = false;
            lock (lockCookie)
            {
                TimeSpan sinceLastSent = DateTime.Now - lastSent;
                if (sinceLastSent > frequency)
                {
                    shouldSend = true;
                }
            }

            if (shouldSend)
            {
                sendTweetDestination(sentence);
                lock (lockCookie)
                {
                    lastSent = DateTime.Now;
                }
            }

        }

        private SpellChecker SetupSpellChecker()
        {
            var hunspell = new Hunspell("en_GB-oed.aff", "en_GB-oed.dic");
            return new SpellChecker(hunspell);
        }
    }
}
