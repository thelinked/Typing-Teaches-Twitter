﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AnalysisLibs;
using MahApps.Metro.Controls;

namespace TwitterTeachesTyping
{
    public partial class MainWindow : MetroWindow
    {
        private readonly GameController controller;
        private int Score { get; set; }

        private List<AnalysedSentence> bufferedTweets;
        private Dictionary<int,AnalysedSentence> activeTweets;

        private bool waitingForTweet;

        public MainWindow()
        {
            InitializeComponent();
            chooseTopic.KeyDown += (sender, e) => DispatchOnEnter(sender,e, OnChooseTopic);
            scoreLabel.Content = "Score:0";
            controller = new GameController(HandleTweet);

            bufferedTweets = new List<AnalysedSentence>();
            activeTweets = new Dictionary<int, AnalysedSentence>();

            waitingForTweet = true;
        }

        private void HandleTweet(AnalysedSentence tweet)
        {
            Dispatcher.BeginInvoke((Action)(() => ReallyHandleTweet(tweet)));
        }

        private void ReallyHandleTweet(AnalysedSentence tweet)
        {
            if (!waitingForTweet)
            {
                bufferedTweets.Add(tweet);
            }
            else
            {
                AddTweetToUI(tweet);
                waitingForTweet = false;
            }
        }
        private void AddTweetToUI(AnalysedSentence tweet)
        {

            var fancyTweet = new Paragraph();
            var original = tweet.Original;
            var needToPrintLast = false;



            //Jack, please fix!
            //This is totally broken
            foreach (var word in tweet.Words.Where(x => !x.Correct))
            {
                //If a misspelt word is in the sentance more than once this will be wrong
                var divided = original.Split(new[] { word.Word }, StringSplitOptions.None);

                if (divided.Count() > 1)
                {
                    fancyTweet.Inlines.Add(new Run(divided[0]));
                    fancyTweet.Inlines.Add(new Run(word.Word) { Background = Brushes.Red });
                    needToPrintLast = true;
                }
                else
                {
                    fancyTweet.Inlines.Add(new Run(divided[0]));
                    needToPrintLast = false;
                    break;
                }

                original = divided[1];
            }
            if (needToPrintLast)
            {
                fancyTweet.Inlines.Add(original);
            }


            var box = new RichTextBox
            {
                Document = new FlowDocument(fancyTweet), 
                Width = 600,
                VerticalAlignment = VerticalAlignment.Center,
                Focusable = true
            };

            var answer = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 120
            };

            var lol = answer.GetHashCode();

            activeTweets.Add(answer.GetHashCode(), tweet);
            answer.KeyDown += (sender, e) => DispatchOnEnter(sender,e, OnPlayerEnteredText);
            var Image = new Image
                {
                    Source = doGetImageSourceFromResource("TwitterTeachesTyping", "question.png"),
                    Width = 56
                };


            var panel = new WrapPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

            panel.Children.Add(box);
            panel.Children.Add(answer);
            panel.Children.Add(Image);

            stackPanel.Children.Insert(0, panel);
        }

        private void OnPlayerEnteredText(object sender)
        {
            var box = (TextBox)sender;
            var playerText = box.Text;

            AnalysedSentence tweet;
            if (activeTweets.TryGetValue(box.GetHashCode(), out tweet))
            {
                foreach (var word in tweet.Words.Where(x => !x.Correct))
                {
                    if (word.Suggestions.Any((x) => x == playerText))
                    {
                        Score += (int)(tweet.StupidityPercentage * 100);
                        
                    }
                }
                activeTweets.Remove(box.GetHashCode());
                scoreLabel.Content = string.Format("Score:{0}", Score);                
            }

            if (!activeTweets.Any())
            {
                waitingForTweet = true;
            }
            else
            {
                waitingForTweet = false;
                AddTweetToUI(tweet);
            }
        }

        static internal ImageSource doGetImageSourceFromResource(string psAssemblyName, string psResourceName)
        {
            Uri oUri = new Uri("pack://application:,,,/" + psAssemblyName + ";component/" + psResourceName, UriKind.RelativeOrAbsolute);
            return BitmapFrame.Create(oUri);
        }

        private void DispatchOnEnter(object sender, KeyEventArgs e, Action<object> onEnter )
        {
            if (e.Key == Key.Enter)
            {
                onEnter(sender);
            }
        }

        private void OnChooseTopic(object sender)
        {
            Console.WriteLine(chooseTopic.Text);

            if (!string.IsNullOrWhiteSpace(chooseTopic.Text))
            {
                controller.Listen(chooseTopic.Text.Split(',').Select(x => x.Trim()).ToArray());
            }
        }
    }
}
