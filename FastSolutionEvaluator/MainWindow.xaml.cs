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
                TextRange range = new TextRange(fileView.Document.ContentStart, fileView.Document.ContentEnd);
            range.Text  = (lbFilesInSLN.SelectedItem as CSFile).Content;
            }
        }

        #region SyntaxHighligting
        private void FileView_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (fileView.Document == null)
                return;

            TextRange documentRange = new TextRange(fileView.Document.ContentStart, fileView.Document.ContentEnd);
            documentRange.ClearAllProperties();
            TextPointer navigator = fileView.Document.ContentStart;
            while (navigator.CompareTo(fileView.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    CheckWordsInRun((Run)navigator.Parent);

                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }
            Format();
        }

        List<Tag> m_tags = new List<Tag>();
        void Format()
        {
            fileView.TextChanged -= this.FileView_OnTextChanged;

            for (int i = 0; i < m_tags.Count; i++)
            {
                TextRange range = new TextRange(m_tags[i].StartPosition, m_tags[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            m_tags.Clear();

            fileView.TextChanged += this.FileView_OnTextChanged;
        }

        void CheckWordsInRun(Run run)
        {
            string text = run.Text;

            int sIndex = 0;
            int eIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]) | CSyntaxProvider.GetSpecials.Contains(text[i]))
                {
                    if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | CSyntaxProvider.GetSpecials.Contains(text[i - 1])))
                    {
                        eIndex = i - 1;
                        string word = text.Substring(sIndex, eIndex - sIndex + 1);

                        if (CSyntaxProvider.IsKnownTag(word))
                        {
                            Tag t = new Tag();
                            t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                            t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                            t.Word = word;
                            m_tags.Add(t);
                        }
                    }
                    sIndex = i + 1;
                }
            }

            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            if (CSyntaxProvider.IsKnownTag(lastWord))
            {
                Tag t = new Tag();
                t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                t.Word = lastWord;
                m_tags.Add(t);
            }
        }
        #endregion
    }
}
