using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace towerDefence
{
    public class AnalysedSentence
    {
        public IEnumerable<AnalysedWord> Words { get; private set; }
        public string Original { get; private set; }
        public bool HasMisspelling { get; private set; }
        public double StupidityPercentage { get; private set; }
        public bool IsValid { get; private set; }

        public AnalysedSentence(string original, IEnumerable<AnalysedWord> words)
        {
            IsValid = true;
            this.Original = original;
            this.Words = words;
            HasMisspelling = words.Any(a => !a.Correct);
            int wordCount = words.Count();
            if (wordCount == 0)
            {
                IsValid = false;
            }
            int misSpellingCount = words.Count(a => !a.Correct);
            StupidityPercentage = Math.Round((double)misSpellingCount / ((double)wordCount) * 100.0);
        }

        public override string ToString()
        {
            string sentence = "";
            foreach (var analysedWord in Words)
            {
                sentence += analysedWord.Word + " ";
            }
            sentence += " stupidity:" + Math.Round(StupidityPercentage, 0);
            return sentence;
        }
    }
}
