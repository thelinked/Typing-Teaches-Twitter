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
        public double Attage { get; private set; }
        public double Linkyness { get; private set; }
        public double HashQuotient { get; private set; }
        public double WordCount { get; private set; }

        public AnalysedSentence(string original, IEnumerable<AnalysedWord> words, double attage, double linkyness, double hashQuotient)
        {
            this.Attage = attage;
            this.HashQuotient = hashQuotient;
            this.Linkyness = linkyness;
            IsValid = true;
            this.Original = original;
            this.Words = words;
            HasMisspelling = words.Any(a => !a.Correct);
            WordCount = words.Count();
            if (WordCount == 0)
            {
                IsValid = false;
            }
            int misSpellingCount = words.Count(a => !a.Correct);
            StupidityPercentage = Math.Round((double)misSpellingCount / ((double)WordCount) * 100.0);
        }

        public override string ToString()
        {
            string sentence = "";
            foreach (var analysedWord in Words)
            {
                sentence += analysedWord.Word + " ";
            }
            sentence += " stupidity:" + Math.Round(StupidityPercentage);
            sentence += " attage:" + Math.Round(Attage);
            sentence += " hashQuotient:" + Math.Round(HashQuotient);
            sentence += " linkyness:" + Math.Round(Linkyness);
            return sentence;
        }

        public string ToCSV()
        {
            return WordCount+","+StupidityPercentage+","+Attage+","+Linkyness+"," + HashQuotient;
        }
    }
}
