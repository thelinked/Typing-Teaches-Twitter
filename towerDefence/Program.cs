using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace towerDefence
{
    class Program
    {
        private static TwitterStream inStream;

        static void Main(string[] args)
        {
            inStream = SetupTwitterStream();
        }

        private static TwitterStream SetupTwitterStream()
        {
            var tagsToTrack = new[] { "#fail,#BeliebersHatePaparazzi" };
            var user = "oxfordDefence";
            var pass = "oxfordTowerDefence";
            var stream = new TwitterStream(user, pass, HandleTweet);
            stream.Stream(tagsToTrack);
            return stream;
        }

        private static void HandleTweet(Status status)
        {
            Console.WriteLine(status.text);
        }
    }
}
