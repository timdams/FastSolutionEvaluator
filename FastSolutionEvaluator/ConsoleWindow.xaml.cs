using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FastSolutionEvaluator
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        public ConsoleWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            //TODO: Save output for future reference
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (PathToProc != null && PathToProc != "")
            {
                consolectrl.IsInputEnabled = true;
                
                consolectrl.StartProcess(PathToProc, "");

                txbInput.Focus();
                
            }
        }
        public string PathToProc { get; set; }

        private void btnWriteToConsole_Click(object sender, RoutedEventArgs e)
        {
            consolectrl.WriteInput(txbInput.Text, Colors.Yellow, true);
            txbInput.Text = "";
            txbInput.Focus();
        }
    }
}
