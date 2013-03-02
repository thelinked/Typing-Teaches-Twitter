using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHunspell;

namespace SpellCheckTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Hunspell hunspell = new Hunspell("en_GB-oed.aff", "en_GB-oed.dic"))
            {
                
                bool correct = hunspell.Spell("Recommendation");
                

                List<string> suggestions = hunspell.Suggest("Recommendatio");
                Console.WriteLine("There are " + suggestions.Count.ToString() +
                                  " suggestions");
                
                foreach (string suggestion in suggestions)
                {
                    Console.WriteLine("Suggestion is: " + suggestion);
                }
            }
        }
    }
}
