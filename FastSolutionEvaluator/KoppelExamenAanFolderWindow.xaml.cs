using FastSolutionEvaluator.ExamBuilderClasses;
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
    /// Interaction logic for KoppelExamenAanFolderWindow.xaml
    /// </summary>
    public partial class KoppelExamenAanFolderWindow : Window
    {
        public string TeKoppelenExamenPath { get; set; } = "";
        public KoppelExamenAanFolderWindow()
        {
            InitializeComponent();
        }

        private void btnOpenExamBuilder_Click(object sender, RoutedEventArgs e)
        {
            ExamBuilderWindow wnd = new ExamBuilderWindow();
            wnd.ShowDialog();
        }

        private void btnLaadExamenEnKoppe_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "XML Files|*.XML";
            if (dlg.ShowDialog().Value == true)
            {
                try
                {
                    TeKoppelenExamenPath = dlg.FileName;
                    this.Close();
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnIgnoreForNow_Click(object sender, RoutedEventArgs e)
        {
            TeKoppelenExamenPath = "";
            this.Close();
        }
    }
}
