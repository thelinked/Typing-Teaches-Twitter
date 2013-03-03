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
using TwitterTeachesTyping.Annotations;

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

        }
        private void chooseTopic_MouseEnter(object sender, RoutedEventArgs e)
        {

        }


        private void OnTargetUpdated(object sender, DataTransferEventArgs dataTransferEventArgs)
        {
            
        }


        public string Name2 { get; set; }
    }
}
