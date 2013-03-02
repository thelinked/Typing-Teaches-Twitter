using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalysisLibs;
using NHunspell;

namespace towerDefence
{
    static class Program
    {
        private static TwitterStream inStream;
        private static SpellChecker spellChecker;
        private static TextWriter csvWriter;

        static void Main(string[] args)
        {
            string stream_url = ConfigurationManager.AppSettings["stream_url"];
            csvWriter = new StreamWriter(@"C:\Temp\twitterlog.csv");
            spellChecker = SetupSpellChecker();
            inStream = SetupTwitterStream(stream_url, new[] { "#BeliebersHatePaparazzi" });
        }

        private static SpellChecker SetupSpellChecker()
        {
            var hunspell = new Hunspell("en_GB-oed.aff", "en_GB-oed.dic");
            return new SpellChecker(hunspell);
        }

        private static TwitterStream SetupTwitterStream(string url, string[] toTrack)
        {
            var language = "en";
            var user = "oxfordDefence";
            var pass = "oxfordTowerDefence";
            var stream = new TwitterStream(url, user, pass, HandleTweet);
            stream.Stream(toTrack,language);
            return stream;
        }

        private static void HandleTweet(Status status)
        {
            AnalysedSentence analysed = spellChecker.CheckSentence(status.text);
            if (analysed.IsValid)
            {
                Console.WriteLine(analysed);
                csvWriter.WriteLine(analysed.ToCSV());
                csvWriter.Flush();
            }
        }
    }
}
