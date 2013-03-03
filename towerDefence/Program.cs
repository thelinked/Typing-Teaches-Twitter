using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AnalysisLibs;
using NHunspell;

namespace towerDefence
{
    static class Program
    {

        static void Main(string[] args)
        {
            GameController controller = new GameController(PrintTweet);

            controller.SetFreqency(new TimeSpan(0,0,0,5));

            controller.Listen(new[] { "#ItMakesMeHappyWhen" });
//
//            Thread.Sleep(10000);
//
//            controller.Listen(new[] { "#BackInTheDayHitBangers" });
//
//            Thread.Sleep(10000);
//            controller.SetFreqency(new TimeSpan(0,0,0,1));
        }

        private static void PrintTweet(AnalysedSentence sentence)
        {
            Console.WriteLine(sentence.Original);
        }

        
    }
}
