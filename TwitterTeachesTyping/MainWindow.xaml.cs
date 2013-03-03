using System;
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

        private readonly Queue<AnalysedSentence> bufferedTweets;
        private readonly Dictionary<int,Tuple<AnalysedSentence,Image,Label>> activeTweets;

        private bool waitingForTweet;

        public MainWindow()
        {
            InitializeComponent();
            chooseTopic.KeyDown += (sender, e) => DispatchOnEnter(sender,e, OnChooseTopic);
            scoreLabel.Content = "Score:0";
            controller = new GameController(HandleTweet);

            bufferedTweets = new Queue<AnalysedSentence>();
            activeTweets = new Dictionary<int, Tuple<AnalysedSentence, Image,Label>>();

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
                bufferedTweets.Enqueue(tweet);
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
                Focusable = true,
                IsEnabled = false
            };

            var playerAnswer = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 120,

            };
            TextboxHelper.SetWatermark(playerAnswer, "Type Here");
            TextboxHelper.SetClearTextButton(playerAnswer, true);


            var image = new Image
                {
                    Source = DoGetImageSourceFromResource("TwitterTeachesTyping", "question.png"),
                    Width = 56
                };

            var answers = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            };

            activeTweets.Add(playerAnswer.GetHashCode(), new Tuple<AnalysedSentence, Image,Label>(tweet, image,answers));
            playerAnswer.KeyDown += (sender, e) => DispatchOnEnter(sender, e, OnPlayerEnteredText);

            var panel = new WrapPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

            panel.Children.Add(box);
            panel.Children.Add(playerAnswer);
            panel.Children.Add(image);
            panel.Children.Add(answers);

            stackPanel.Children.Insert(0, panel);
        }

        private void OnPlayerEnteredText(object sender)
        {
            var box = (TextBox)sender;
            var playerText = box.Text;

            Tuple<AnalysedSentence,Image,Label> tweet;
            if (activeTweets.TryGetValue(box.GetHashCode(), out tweet))
            {
                foreach (var word in tweet.Item1.Words.Where(x => !x.Correct))
                {
                    if (word.Suggestions.Any(x => x == playerText))
                    {
                        Score += (int)(tweet.Item1.StupidityPercentage * 100);
                        tweet.Item2.Source = DoGetImageSourceFromResource("TwitterTeachesTyping", "tick.jpg");
                        tweet.Item3.Content = string.Join(",", word.Suggestions);
                        break;
                    }

                    tweet.Item2.Source = DoGetImageSourceFromResource("TwitterTeachesTyping", "x.jpg");
                    tweet.Item3.Content = string.Join(",", word.Suggestions);
                }

                box.IsEnabled = false;

                activeTweets.Remove(box.GetHashCode());
                scoreLabel.Content = string.Format("Score:{0}", Score);                
            }

            if (bufferedTweets.Any())
            {
                waitingForTweet = false;
                AddTweetToUI(bufferedTweets.Dequeue());
            }
            else
            {
                waitingForTweet = true;
            }
        }

        static internal ImageSource DoGetImageSourceFromResource(string psAssemblyName, string psResourceName)
        {
            var oUri = new Uri("pack://application:,,,/" + psAssemblyName + ";component/" + psResourceName, UriKind.RelativeOrAbsolute);
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

            bufferedTweets.Clear();
            waitingForTweet = true;

            if (!string.IsNullOrWhiteSpace(chooseTopic.Text))
            {
                controller.Listen(chooseTopic.Text.Split(',').Select(x => x.Trim()).ToArray());
            }
        }
    }
}
