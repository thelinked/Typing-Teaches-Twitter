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
using MahApps.Metro.Controls;

namespace TwitterTeachesTyping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            btnRaiseEvent.Click += btnRaiseEventCustomArgs_Click;
            chooseTopic.TextInput += chooseTopic_MouseEnter;
        }


        private void btnRaiseEventCustomArgs_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("buttonclick");
        }
        private void chooseTopic_MouseEnter(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("mouseEnter");
        }


        private void OnTargetUpdated(object sender, DataTransferEventArgs dataTransferEventArgs)
        {
            Console.WriteLine("targetUpdated");
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonOK_Click(this, new RoutedEventArgs());
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(chooseTopic.Text);
            // May do other stuff.
            //this.Close();
        }


        public string Name2 { get; set; }
    }
}
