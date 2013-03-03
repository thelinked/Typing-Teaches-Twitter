using System;
using System.Collections.Generic;
using System.Globalization;
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

            var sentence = tweet.Split(new[]{' ',',','(',')','[',']','{','}','&',';','.','\n','\r','~'});

            var realWords = new List<string>();

            foreach (var item in sentence)
            {
                if (item.Contains('\"'))
                {
                    Console.WriteLine("");
                }
                var word = item.Replace("\"", "");
                word = replaceTrailingCharacters(word);
                if (word.StartsWith("rt",true,CultureInfo.CurrentCulture))
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
                var plain = rgx.Replace(item, "");
                if (string.IsNullOrWhiteSpace(plain))
                {
                    continue;
                }
                if (word.Contains('/'))
                {
                    var splitSlash = word.Split('/');
                    foreach (var s in splitSlash)
                    {
                        realWords.Add(s);
                    }
                }
                else
                {
                    realWords.Add(word);
                }
                
            }
            foreach (var realWord in realWords)
            {
                List<string> suggestions;
                if (!IsException(realWord))
                {
                    var correct = CheckWord(realWord, out suggestions);
                    AnalysedWord analysedWord;
                    if (!correct)
                    {
                        //if no suggestions, just mark as being correct.
                        if (!suggestions.Any())
                        {
                            analysedWord = new AnalysedWord()
                                {
                                    Correct = true,
                                    Suggestions = suggestions,
                                    Word = realWord
                                };
                        }
                        else
                        {
                            analysedWord = new AnalysedWord()
                            {
                                Correct = false,
                                Suggestions = suggestions,
                                Word = realWord
                            };
                        }


                    }
                    else
                    {
                        analysedWord = new AnalysedWord()
                        {
                            Correct = true,
                            Suggestions = suggestions,
                            Word = realWord
                        };
                    }
                    analysedWords.Add(analysedWord);
                }
                else
                {
                    analysedWords.Add(new AnalysedWord(){Correct = true,Suggestions = new List<string>(),Word = realWord});
                }
                
            }
            return new AnalysedSentence(tweet,analysedWords,attage,linkiness,hashQuotient);
        }

        private string replaceTrailingCharacters(string original)
        {
            return original.TrimEnd(new[] {'!', '?', ';', ':'});
        }

        private bool IsException(string item)
        {
            var word = item.ToLower();
            if (word.Length < 3)
            {
                return true;
            }

            List<string> exceptions = new List<string>()
                {
                    "i","omg","omfg","lol"
                };

            return exceptions.Contains(word);
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
