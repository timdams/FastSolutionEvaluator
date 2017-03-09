using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
using Microsoft.CSharp;
using Microsoft.Win32;
using System.Windows.Markup;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ICSharpCode.AvalonEdit.Highlighting;
using FastSolutionEvaluator.utility.msbuild;
using FastSolutionEvaluator.ViewModel;

namespace FastSolutionEvaluator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string evalfilepath = Properties.Settings.Default.LastPath + "\\evalsresults.csv";
        public MainWindow()
        {
            InitializeComponent();

        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            dialog.SelectedPath = Properties.Settings.Default.LastPath;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                Properties.Settings.Default.LastPath = dialog.SelectedPath;
                Properties.Settings.Default.Save();

                var allslns = new List<SolutionVM>();

                lbSLNS.ItemsSource = SolutionsVM.Load(dialog.SelectedPath);
            }
            //TODO: implement in new eval datamode
            //if (File.Exists(evalfilepath))
            //{  //Update eval status
            //    string logfile = File.ReadAllText(evalfilepath);
            //    foreach (var solution in allslns)
            //    {
            //        if (GetLineNumber(logfile, solution.FolderName) > -1)
            //            solution.IsEvaled = true;
            //    }
            //}
            //Show it




        }

        private void btnStartDebugExe_Click(object sender, RoutedEventArgs e)
        {
            if (lbSLNS.SelectedIndex != -1)
            {
                ProjectVM selsol = null;
                //MessageBox.Show((lbSLNS.SelectedItem as SolutionMeta).FullPath+"\\"+lbPROJS.SelectedItem + "\\bin\\debug\\"+lbPROJS.SelectedItem+".exe");
                //
                if (lbPROJS.SelectedIndex != -1)
                    selsol = (lbPROJS.SelectedItem as ProjectVM);
                else
                    selsol = (lbSLNS.SelectedItem as SolutionVM).Projects.First();//Using first project
                if (selsol != null)
                {
                    if (selsol.BestExePath != "null")
                        try
                        {
                            var Proc = Process.Start(selsol.BestExePath);
                        }
                        catch (Exception ex)
                        {
                            lbLog.Items.Insert(0, string.Format(ex.Message));
                        }
                    else { lbLog.Items.Insert(0, "No build exe found"); }
                }

            }
        }



        private void btnOpenInVS_Click(object sender, RoutedEventArgs e)
        {
            if (lbSLNS.SelectedIndex != -1)
            {

                //MessageBox.Show((lbSLNS.SelectedItem as SolutionMeta).FullPath+"\\"+lbPROJS.SelectedItem + "\\bin\\debug\\"+lbPROJS.SelectedItem+".exe");
                //
                string debugp = (lbSLNS.SelectedItem as SolutionVM).PathToSln;
                try
                {
                    Process.Start(debugp);
                }
                catch (Exception ex)
                {
                    lbLog.Items.Insert(0, ex.Message);
                }
            }
        }

        private void trycompileandRun_Click(object sender, RoutedEventArgs e)
        {
            if (lbSLNS.SelectedIndex != -1)
            {
                ProjectVM selsol = null;
                //MessageBox.Show((lbSLNS.SelectedItem as SolutionMeta).FullPath+"\\"+lbPROJS.SelectedItem + "\\bin\\debug\\"+lbPROJS.SelectedItem+".exe");
                //
                if (lbPROJS.SelectedIndex != -1)
                    selsol = (lbPROJS.SelectedItem as ProjectVM);
                else
                    //TODO: allow user to choose what project(s) to build
                    selsol = (lbSLNS.SelectedItem as SolutionVM).Projects.First();//Using first project
                if (selsol != null)
                {
                    //TODO find msbuild (add to program?)
                    string temppath = Environment.CurrentDirectory + "\\temp";
                    //Clean temp folder:
                    Directory.Delete(temppath, true); //TODO: is dit veilig???!
                    string mspath = $"\"C:\\Program Files (x86)\\MSBuild\\12.0\\Bin\\msbuild.exe\"  \"{selsol.Project.Project.FullPath}\" /p:OutDir=\"{temppath}\"";
                    //string mspath = "msbuild";


                    var sb = new StringBuilder();

                    Process p = new Process();

                    // redirect the output
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;

                    // hookup the eventhandlers to capture the data that is received
                    p.OutputDataReceived += (senderp, args) => sb.AppendLine(args.Data);
                    p.ErrorDataReceived += (senderp, args) => sb.AppendLine(args.Data);

                    // direct start
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.FileName = mspath;
                    //  p.StartInfo.Arguments = $"{selsol.Project.Project.FullPath}";
                    p.Start();
                    // start our event pumps
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();

                    // until we are done
                    p.WaitForExit();

                    //TODO: Write sb to logfile
                    if (sb.ToString().Contains("Build succeeded"))
                    {
                        if (MessageBoxResult.Yes == MessageBox.Show("Build successfull. Do you wish to run the compiled exe?", "Succes", MessageBoxButton.YesNo, MessageBoxImage.Question))
                        {
                            var files= Directory.GetFiles(temppath);
                            foreach (var file in files)
                            {
                                if (file.EndsWith("exe"))
                                    Process.Start(file);//TODO: console screen will dissapear immediately
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("FAIL:" + sb.ToString().Split(new string[] { "Build FAILED" }, StringSplitOptions.None).Last(), "FAILURE",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                    }

                }


            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void writeResult_Click(object sender, RoutedEventArgs e)
        {
            if (lbSLNS.SelectedIndex != -1)
            {
                string result = (lbSLNS.SelectedItem as SolutionMeta).FolderName;
                result += ";";
                result += (chkbUI.IsChecked == true) ? "1;" : "0;";
                result += (chkbUI2.IsChecked == true) ? "1;" : "0;";
                result += (chkbFlow1.IsChecked == true) ? "1;" : "0;";
                result += (chkbFlow2.IsChecked == true) ? "1;" : "0;";
                result += (chkbFlow3.IsChecked == true) ? "1;" : "0;";
                result += (chkbMethode1.IsChecked == true) ? "1;" : "0;";
                result += (chkbMethode2.IsChecked == true) ? "1;" : "0;";
                result += (chkbMethode3.IsChecked == true) ? "1;" : "0;";
                result += (chkbMethode4.IsChecked == true) ? "1;" : "0;";
                result += (chkbMethode5.IsChecked == true) ? "1;" : "0;";
                result += (chkbMethode6.IsChecked == true) ? "1;" : "0;";
                result += (chkbMethode7.IsChecked == true) ? "1;" : "0;";
                result += (chkbLoop1.IsChecked == true) ? "1;" : "0;";
                result += (chkbReset1.IsChecked == true) ? "1;" : "0;";
                result += (chkbReset2.IsChecked == true) ? "1;" : "0;";
                result += (chkbReset3.IsChecked == true) ? "1;" : "0;";
                result += (chkbReset4.IsChecked == true) ? "1;" : "0;";
                result += (chkbPro1.IsChecked == true) ? "1;" : "0;";
                result += (chkbPro2.IsChecked == true) ? "1;" : "0;";

                result += DateTime.Now;
                MessageBox.Show(result);

                WriteEvalStuffToFile(result, (lbSLNS.SelectedItem as SolutionMeta));
                ResetState();
                evalstuffchanged = false;
            }
            else
                MessageBox.Show("Selecteer eerst solution");
        }

        private void WriteEvalStuffToFile(string result, SolutionMeta solution)
        {

            if (File.Exists(evalfilepath))
            {
                //TODO: write stuff away
                int line = GetLineNumber(File.ReadAllText(evalfilepath), solution.FolderName.ToString());
                if (line != -1)
                {
                    if (MessageBox.Show("bestaat al... now what? Overwrite?", "Oei", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        File_DeleteLine(line, evalfilepath);
                        WriteResultLine(result, evalfilepath, solution);

                    }
                }
                {
                    WriteResultLine(result, evalfilepath, solution);
                }
            }
            else
            {
                WriteResultLine(result, evalfilepath, solution);
            }
        }

        private static void WriteResultLine(string result, string evalfilepath, SolutionMeta sol)
        {
            var f = File.AppendText(evalfilepath);
            f.WriteLine(result);
            f.Close();
            sol.IsEvaled = true;


        }

        void File_DeleteLine(int Line, string Path)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(Path))
            {
                int Countup = 0;
                while (!sr.EndOfStream)
                {
                    Countup++;
                    if (Countup != Line)
                    {
                        using (StringWriter sw = new StringWriter(sb))
                        {
                            sw.WriteLine(sr.ReadLine());
                        }
                    }
                    else
                    {
                        sr.ReadLine();
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(Path))
            {
                sw.Write(sb.ToString());
            }
        }
        public int GetLineNumber(string text, string lineToFind, StringComparison comparison = StringComparison.CurrentCulture)
        {
            int lineNum = 0;
            using (StringReader reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNum++;
                    if (line.Contains(lineToFind))
                        return lineNum;
                }
            }
            return -1;
        }

        private void ResetState()
        {
            chkbUI.IsChecked = false;
            chkbUI2.IsChecked = false;
            chkbFlow1.IsChecked = false;
            chkbFlow2.IsChecked = false;
            chkbFlow3.IsChecked = false;
            chkbMethode1.IsChecked = false;
            chkbMethode2.IsChecked = false;
            chkbMethode3.IsChecked = false;
            chkbMethode4.IsChecked = false;
            chkbMethode5.IsChecked = false;
            chkbMethode6.IsChecked = false;
            chkbMethode7.IsChecked = false;
            chkbLoop1.IsChecked = false;
            chkbReset1.IsChecked = false;
            chkbReset2.IsChecked = false;
            chkbReset3.IsChecked = false;
            chkbReset4.IsChecked = false;
            chkbPro1.IsChecked = false;
            chkbPro2.IsChecked = false;
        }

        bool evalstuffchanged = false;
        private void chkbUI_Click(object sender, RoutedEventArgs e)
        {
            evalstuffchanged = true;
        }

        private void lbFilesInProj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbFilesInProj.SelectedIndex != -1)
            {
                //AvalaonEdit not very bindable :/
                fileView.Load((lbFilesInProj.SelectedItem as FileVM).Path);
                fileView.SyntaxHighlighting = (lbFilesInProj.SelectedItem as FileVM).ViewerType;
            }
        }

        private void OpenSolutionInVS_Click(object sender, RoutedEventArgs e)
        {
            if (lbSLNS.SelectedItem != null)
                Process.Start((lbSLNS.SelectedItem as SolutionVM).PathToSln);
        }

        private void OpenSolutionInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (lbSLNS.SelectedItem != null)
                Process.Start(System.IO.Path.GetDirectoryName((lbSLNS.SelectedItem as SolutionVM).PathToSln));
        }

        private void OpenProjectInVS_Click(object sender, RoutedEventArgs e)
        {
            if (lbPROJS.SelectedItem != null)
                Process.Start((lbPROJS.SelectedItem as ProjectVM).Project.Project.FullPath);
        }
        private void OpenProjectInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (lbPROJS.SelectedItem != null)
                Process.Start((lbPROJS.SelectedItem as ProjectVM).Project.Project.DirectoryPath);
        }

        private void OpenFileInDefault_Click(object sender, RoutedEventArgs e)
        {
            if (lbFilesInProj.SelectedItem != null)
                Process.Start((lbFilesInProj.SelectedItem as FileVM).Path);
        }

        private void OpenFileInNotepad_Click(object sender, RoutedEventArgs e)
        {
            if (lbFilesInProj.SelectedItem != null)
                Process.Start("notepad.exe", (lbFilesInProj.SelectedItem as FileVM).Path);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    OpenSolutionInVS_Click(this, null);
                    break;

            }
        }
    }




}
