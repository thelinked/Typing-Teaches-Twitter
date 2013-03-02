using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHunspell;

namespace towerDefence
{
    static class Program
    {
        private static TwitterStream inStream;
        private static SpellChecker spellChecker;

        static void Main(string[] args)
        {
            spellChecker = SetupSpellChecker();
            inStream = SetupTwitterStream();
        }

        private static SpellChecker SetupSpellChecker()
        {
            var hunspell = new Hunspell("en_GB-oed.aff", "en_GB-oed.dic");
            return new SpellChecker(hunspell);
        }

        private static TwitterStream SetupTwitterStream()
        {
            var tagsToTrack = new[] { "#fail,#BeliebersHatePaparazzi" };
            var language = "en";
            var user = "oxfordDefence";
            var pass = "oxfordTowerDefence";
            var stream = new TwitterStream(user, pass, HandleTweet);
            stream.Stream(tagsToTrack,language);
            return stream;
        }

        private static void HandleTweet(Status status)
        {
            AnalysedSentence analysed = spellChecker.CheckSentence(status.text);
            if (analysed.IsValid && analysed.HasMisspelling)
            {
                if (analysed.StupidityPercentage > 50)
                {
                    Console.WriteLine(analysed);
                }
                
            }
        }
    }
}
