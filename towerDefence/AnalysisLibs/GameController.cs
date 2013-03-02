using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisLibs
{
    public class GameController
    {
        public delegate void SubscribeToTopic(string topic);

        public delegate void TweetFrequency(int millis);

        public delegate void DifficultyLevel(int difficulty);

        public delegate void SendTweet(AnalysedSentence sentence);
    }
}
