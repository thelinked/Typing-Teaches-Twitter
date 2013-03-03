using System;
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

        private readonly ScoreTracker scoreTracker;
        private readonly Random rand;

        public MainWindow()
        {
            InitializeComponent();
            InitializeComponent();
            chooseTopic.TextInput += chooseTopic_MouseEnter;
            chooseTopic.KeyDown += (sender, e) => DispatchOnEnter(e, OnChooseTopic);
            scoreLabel.Content = "Score:0";
            controller = new GameController(HandleTweet);
            scoreTracker = new ScoreTracker();
            rand = new Random();
        }

        private void HandleTweet(AnalysedSentence tweet)
        {
            scoreTracker.Score = (rand.Next() * 100);

           

            Dispatcher.BeginInvoke((Action)(() =>
                {
                    scoreLabel.Content = string.Format("Score:{0}", scoreTracker.Score);

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
                    };


                    var answer = new TextBox
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 120
                    };
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


     
                   // <TextBox x:Name="Guess" TextWrapping="Wrap" Text="TextBox" VerticalAlignment="Center" Width="120"/

                    stackPanel.Children.Insert(0, panel);
                }));


        }

        static internal ImageSource doGetImageSourceFromResource(string psAssemblyName, string psResourceName)
        {
            Uri oUri = new Uri("pack://application:,,,/" + psAssemblyName + ";component/" + psResourceName, UriKind.RelativeOrAbsolute);
            return BitmapFrame.Create(oUri);
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
