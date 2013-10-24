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
            var allslns = new List<SolutionMeta>();
            foreach (var directory in Directory.GetDirectories("TEST"))
            {

                var sln = new SolutionMeta();
                sln.StudentName = directory.ToLower().Replace("examen", "").Split('\\').Last();

                //Find .sln file
                var res = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
                foreach (var re in res)
                {
                    var cs = new CSFile();
                    cs.FileName = re.Split('\\').Last();
                    if (cs.FileName.Contains("AssemblyInfo") || cs.FileName.Contains("TemporaryGeneratedFile"))
                        continue;
                    cs.Content=  File.ReadAllText(re);
                    sln.CSFiles.Add(cs);
                }
                   //Sort
              sln.CSFiles=  sln.CSFiles.OrderByDescending(p => p.FileName.ToLower().Contains("program.cs")).ThenBy(p => p.FileName).ToList();
               


                allslns.Add(sln);
            }

            lbSLNS.ItemsSource = allslns;

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
        public string StudentName { get; set; }
        public string ErrorInfo { get; set; }
        public override string ToString()
        {
            return StudentName + "(" + ErrorInfo + ")";
        }

        public List<CSFile> CSFiles { get; set; }


        public SolutionMeta()
        {
            CSFiles= new List<CSFile>();
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
