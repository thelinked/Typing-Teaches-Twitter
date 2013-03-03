using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AnalysisLibs;
using MahApps.Metro.Controls;

namespace TwitterTeachesTyping
{
    public partial class MainWindow : MetroWindow
    {
        private readonly GameController controller;

        private readonly ScoreTracker scoreTracker;
        private readonly Random rand;

        public MainWindow()
        {
            InitializeComponent();
            InitializeComponent();
            chooseTopic.TextInput += chooseTopic_MouseEnter;
            chooseTopic.KeyDown += (sender, e) => DispatchOnEnter(e, OnChooseTopic);
           // scoreLabel.Content = "Score:0";
            controller = new GameController(HandleTweet);
        }

        private void HandleTweet(AnalysedSentence tweet)
        {
            this.Dispatcher.BeginInvoke(
            (Action)delegate()
            {
                stackPanel.Children.Insert(0, new TextBox() { Text = tweet.Original });
            });
            scoreTracker.Score = (int)(rand.Next() * 100);
            //scoreLabel.Content = string.Format("Score:{0}", scoreTracker.Score);
        }

        private void chooseTopic_MouseEnter(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("mouseEnter");
        }

        private void DispatchOnEnter(KeyEventArgs e, Action onEnter )
        {
            if (e.Key == Key.Enter)
            {
                onEnter();
            }
        }
        private void OnChooseTopic()
        {
            Console.WriteLine(chooseTopic.Text);

            if (!string.IsNullOrWhiteSpace(chooseTopic.Text))
            {
                controller.Listen(chooseTopic.Text.Split(',').Select(x => x.Trim()).ToArray());
            }
        }
    }
}
