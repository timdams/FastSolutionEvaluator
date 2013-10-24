using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace FastSolutionEvaluator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                var allslns = new List<SolutionMeta>();
                foreach (var directory in Directory.GetDirectories(dialog.SelectedPath))
                {

                    var sln = new SolutionMeta();
                    sln.FolderName = directory;

                    //Find .sln file
                    var res = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
                    foreach (var re in res)
                    {
                        var cs = new CSFile();
                        cs.FileName = re.Split('\\').Last();
                        if (cs.FileName.Contains("AssemblyInfo") || cs.FileName.Contains("TemporaryGeneratedFile"))
                            continue;
                        cs.Content = File.ReadAllText(re);
                        sln.CSFiles.Add(cs);
                    }
                    //Sort
                    sln.CSFiles =
                        sln.CSFiles.OrderByDescending(p => p.FileName.ToLower().Contains("program.cs"))
                           .ThenBy(p => p.FileName)
                           .ToList();



                    allslns.Add(sln);
                }

                lbSLNS.ItemsSource = allslns;
            }
        }

        private void LbSLNS_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbSLNS.SelectedIndex != -1)
            {
                lbFilesInSLN.ItemsSource = (lbSLNS.SelectedItem as SolutionMeta).CSFiles;
                if (lbFilesInSLN.Items.Count > 0)
                    lbFilesInSLN.SelectedIndex = 0;
            }
        }

        private void LbFilesInSLN_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbFilesInSLN.SelectedIndex != -1)
            {
                fileView.Text = (lbFilesInSLN.SelectedItem as CSFile).Content;
            }
        }
    }

    class SolutionMeta
    {
        public string FolderName { get; set; }

        public override string ToString()
        {
            return FolderName;
        }

        public List<CSFile> CSFiles { get; set; }


        public SolutionMeta()
        {
            CSFiles = new List<CSFile>();
        }
    }
    class CSFile
    {
        public string FileName { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return FileName;
        }
    }
}
