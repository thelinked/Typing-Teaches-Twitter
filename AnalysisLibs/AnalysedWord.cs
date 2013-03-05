using System.Collections.Generic;

namespace AnalysisLibs
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
