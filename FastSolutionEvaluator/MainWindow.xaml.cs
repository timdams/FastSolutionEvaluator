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

    private void LbSLNS_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        //if (lbSLNS.SelectedIndex != -1)
        //{
        //    lbPROJS.ItemsSource = (lbSLNS.SelectedItem as SolutionMeta).Csprojs;
        //    lbLog.Items.Insert(0, (lbSLNS.SelectedItem as SolutionMeta).FullPath);
        //    if (lbPROJS.Items.Count > 0)
        //        lbPROJS.SelectedIndex = 0;
        //}
    }

    private void LbPROJS_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lbPROJS.SelectedIndex != -1)
        {
            lbFilesInSLN.ItemsSource = (lbPROJS.SelectedItem as CSPROJ).CSFiles;
            if (lbFilesInSLN.Items.Count > 0)
                lbFilesInSLN.SelectedIndex = 0;
        }
    }

    private void LbFilesInSLN_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lbFilesInSLN.SelectedIndex != -1)
        {
            //https://github.com/icsharpcode/AvalonEdit/blob/master/ICSharpCode.AvalonEdit/Highlighting/Resources/Resources.cs
            // http://midnightprogrammer.net/post/Syntax-Highlighter-In-WPF/
            fileView.Load((lbFilesInSLN.SelectedItem as CSFile).Path);
            if (((lbFilesInSLN.SelectedItem as CSFile).FileName.EndsWith("xaml")))
                fileView.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
            else
                fileView.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
        }
    }


    private void TryRunBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (lbFilesInSLN.SelectedIndex != -1)
        {
            //Here comes silly code
            CodeDomProvider comp = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters cp = new CompilerParameters();

            // Generate an executable instead of  
            // a class library.
            cp.GenerateExecutable = true;

            // Set the assembly file name to generate.
            cp.OutputAssembly = "test.exe";

            // Generate debug information.
            cp.IncludeDebugInformation = true;

            // Add an assembly reference.
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");

            // Save the assembly as a physical file.
            cp.GenerateInMemory = false;

            // Set the level at which the compiler  
            // should start displaying warnings.
            cp.WarningLevel = 3;

            // Set whether to treat all warnings as errors.
            cp.TreatWarningsAsErrors = false;

            // Set compiler argument to optimize output.
            cp.CompilerOptions = "/optimize";

            // Set a temporary files collection. 
            // The TempFileCollection stores the temporary files 
            // generated during a build in the current directory, 
            // and does not delete them after compilation.
            cp.TempFiles = new TempFileCollection(".", true);
            try
            {
                var file = lbFilesInSLN.SelectedItem as CSFile;

                var res = comp.CompileAssemblyFromFile(cp, file.Path);
                if (res.Errors.Count > 0)
                {
                    // Display compilation errors.
                    lbLog.Items.Insert(0, string.Format("Errors building {0}", res.PathToAssembly));
                    foreach (CompilerError ce in res.Errors)
                    {
                        lbLog.Items.Insert(0, string.Format("  {0}", ce.ToString()));

                    }
                }
                else
                {
                    lbLog.Items.Insert(0, string.Format(string.Format(" built into {0} successfully.", res.PathToAssembly)));
                    System.Diagnostics.Process.Start(res.PathToAssembly);

                }
            }
            catch (Exception ex)
            {
                lbLog.Items.Insert(0, string.Format(ex.Message));

            }
        }
        else
        {
            MessageBox.Show("No file selected");
        }
    }

    private void btnStartDebugExe_Click(object sender, RoutedEventArgs e)
    {
        if (lbSLNS.SelectedIndex != -1)
        {

            //MessageBox.Show((lbSLNS.SelectedItem as SolutionMeta).FullPath+"\\"+lbPROJS.SelectedItem + "\\bin\\debug\\"+lbPROJS.SelectedItem+".exe");
            //
            SolutionMeta selsol = (lbSLNS.SelectedItem as SolutionMeta);


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



    private void btnOpenInVS_Click(object sender, RoutedEventArgs e)
    {
        if (lbSLNS.SelectedIndex != -1)
        {

            //MessageBox.Show((lbSLNS.SelectedItem as SolutionMeta).FullPath+"\\"+lbPROJS.SelectedItem + "\\bin\\debug\\"+lbPROJS.SelectedItem+".exe");
            //
            string debugp = (lbSLNS.SelectedItem as SolutionMeta).FullPath + "\\" + (lbSLNS.SelectedItem as SolutionMeta).FolderName + ".sln";
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
        // http://stackoverflow.com/questions/22227804/build-visual-studio-solution-from-code
        // https://bogdangavril.wordpress.com/2012/03/15/take-control-of-msbuild-using-msbuild-api/
        string projectFilePath = (lbSLNS.SelectedItem as SolutionMeta).FullPath + "\\" + (lbSLNS.SelectedItem as SolutionMeta).FolderName + ".sln";

        ProjectCollection pc = new ProjectCollection();

        // THERE ARE A LOT OF PROPERTIES HERE, THESE MAP TO THE MSBUILD CLI PROPERTIES
        Dictionary<string, string> globalProperty = new Dictionary<string, string>();
        globalProperty.Add("OutputPath", @"c:\temp");

        BuildParameters bp = new BuildParameters(pc);
        MSBuildLogger customLogger = new MSBuildLogger();
        bp.Loggers = new List<ILogger>() { customLogger };
        BuildRequestData buildRequest = new BuildRequestData(projectFilePath, globalProperty, "14.0", new string[] { "Build" }, null);
        // THIS IS WHERE THE MAGIC HAPPENS - IN PROCESS MSBUILD
        BuildResult buildResult = BuildManager.DefaultBuildManager.Build(bp, buildRequest);
        // A SIMPLE WAY TO CHECK THE RESULT
        if (buildResult.OverallResult == BuildResultCode.Success)
        {
            Process.Start("c:\\temp\\" + (lbSLNS.SelectedItem as SolutionMeta).FolderName + ".exe");


        }
        else
        {
            lbLog.Items.Insert(0, customLogger.BuildErrors);
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
}




}
