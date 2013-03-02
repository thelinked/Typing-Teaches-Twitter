using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnalysisLibs;
using NHunspell;

namespace towerDefence
{
    public class SpellChecker
    {
        private Hunspell hunspell;

        public SpellChecker(Hunspell hunspell)
        {
            this.hunspell = hunspell;
        }

        private double MeasureAttagePercentage(string tweet)
        {
            Regex rgx = new Regex(@"@\w+");

            int length = 0;
            foreach (var match in rgx.Matches(tweet))
            {
                length += match.ToString().Length;
            }
            
            return ((double)length)/((double)tweet.Length)*100.0;
        }

        private double MeasureHashQuotientPercentage(string tweet)
        {
            Regex rgx = new Regex(@"#\w+");

            int length = 0;
            foreach (var match in rgx.Matches(tweet))
            {
                length += match.ToString().Length;
            }

            return ((double)length) / ((double)tweet.Length) * 100.0;
        }

        private double MeasureLinkiness(string tweet)
        {
            Regex rgx = new Regex(@"http://t.co/\w+");

            int length = 0;
            foreach (var match in rgx.Matches(tweet))
            {
                length += match.ToString().Length;
            }

            return ((double)length) / ((double)tweet.Length) * 100.0;
        }

        public AnalysedSentence CheckSentence(string tweet)
        {
            List<AnalysedWord> analysedWords = new List<AnalysedWord>();
            var linkiness = MeasureLinkiness(tweet);
            var attage = MeasureAttagePercentage(tweet);
            var hashQuotient = MeasureHashQuotientPercentage(tweet);

            var sentence = tweet.Split(' ');

            var realWords = new List<string>();

            foreach (var word in sentence)
            {
                if (word.StartsWith("RT"))
                {
                    continue;
                }
                if (word.StartsWith("@"))
                {
                    continue;
                }
                if (word.StartsWith("http") || word.StartsWith("https"))
                {
                    continue;
                }
                if (word.StartsWith("#"))
                {
                    continue;
                }
                Regex rgx = new Regex("[^a-zA-Z]");
                var plain = rgx.Replace(word, "");
                if (string.IsNullOrWhiteSpace(plain))
                {
                    continue;
                }
                realWords.Add(plain);
            }
            foreach (var realWord in realWords)
            {
                List<string> suggestions;
                var correct = CheckWord(realWord, out suggestions);
                var analysedWord = new AnalysedWord() {Correct = correct, Suggestions = suggestions,Word = realWord};
                analysedWords.Add(analysedWord);
            }
            return new AnalysedSentence(tweet,analysedWords,attage,linkiness,hashQuotient);
        }

        private bool CheckWord(string word, out List<string> suggestions)
        {
            List<string> reccomendations;
            var isCorrect = hunspell.Spell(word);

            if (!isCorrect)
            {
                reccomendations = hunspell.Suggest(word);
            }
            else
            {
                reccomendations = new List<string>();
            }
            suggestions = reccomendations;
            return isCorrect;
        }
    }
}
