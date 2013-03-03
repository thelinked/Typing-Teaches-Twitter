using AnalysisLibs;

namespace TwitterTeachesTyping
{
    public class ScoreTracker
    {
        public int Score { get; set; }
        void OnUserInput(AnalysedSentence sentence, string misspelt, string userGuess)
        {
            int deltaScore = 0;
            foreach (var analysedWord in sentence.Words)
            {
                if (analysedWord.Suggestions.Contains(userGuess))
                {
                    deltaScore += (int)(sentence.StupidityPercentage*100);
                }
            }
            Score += deltaScore;
        }
    }
}