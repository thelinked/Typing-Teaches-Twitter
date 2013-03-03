using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AnalysisLibs;
using MahApps.Metro.Controls;

namespace TwitterTeachesTyping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private GameController controller;
        public MainWindow()
        {
            InitializeComponent();
            btnRaiseEvent.Click += btnRaiseEventCustomArgs_Click;
            chooseTopic.TextInput += chooseTopic_MouseEnter;
            chooseTopic.KeyDown += (sender, e) => DispatchOnEnter(e, OnChooseTopic);

            controller = new GameController(PrintTweet);
        }

        private static void PrintTweet(AnalysedSentence sentence)
        {
            Console.WriteLine(sentence.Original);
        }

        private void btnRaiseEventCustomArgs_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("buttonclick");   
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





        private void OnTargetUpdated(object sender, DataTransferEventArgs dataTransferEventArgs)
        {
            Console.WriteLine("targetUpdated");
        }


        public string Name2 { get; set; }
    }
}
