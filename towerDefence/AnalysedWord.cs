using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace towerDefence
{
    public class AnalysedWord
    {
        public string Word { get; set; }
        public bool Correct { get; set; }
        public List<string> Suggestions { get; set; }

        public override string ToString()
        {
            if (Correct)
            {
                return Word + " is ok!";
            }
            else
            {
                string cat = "";
                foreach (var sug in Suggestions)
                {
                    cat += sug + ",";
                }
                return Word + " WRONG. Maybe: " + cat;
            }
        }
    }
}
