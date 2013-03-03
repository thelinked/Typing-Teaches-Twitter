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
//        public delegate void SubscribeToTopic(string topic);
//
//        public delegate void TweetFrequency(int millis);
//
//        public delegate void DifficultyLevel(int difficulty);

        public delegate void SendTweetDelegate(AnalysedSentence sentence);

        private Thread listenerThread;
        private TwitterStream storedStream;
        private SpellChecker spellChecker;
        private SendTweetDelegate sendTweetDestination;

        public GameController(SendTweetDelegate sendTweetDestination)
        {
            this.sendTweetDestination = sendTweetDestination;
            string stream_url = ConfigurationManager.AppSettings["stream_url"];
            storedStream = SetupTwitterStream(stream_url);
            spellChecker = SetupSpellChecker();
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
            var user = "oxfordDefence";
            var pass = "oxfordTowerDefence";
            return new TwitterStream(url, user, pass, AnalyseTweet, language);
        }

        private void AnalyseTweet(Status status)
        {
            AnalysedSentence analysed = spellChecker.CheckSentence(status.text);
            if (analysed.IsValid && analysed.HasMisspelling)
            {
                if (sendTweetDestination != null)
                {
                    sendTweetDestination(analysed);
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
