﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using Microsoft.CSharp;
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

            dialog.SelectedPath = Properties.Settings.Default.LastPath;
            
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                Properties.Settings.Default.LastPath = dialog.SelectedPath;
                Properties.Settings.Default.Save();
        
                var allslns = new List<SolutionMeta>();
                foreach (var directory in Directory.GetDirectories(dialog.SelectedPath, "*.*", SearchOption.AllDirectories))
                {
                    if (Directory.GetFiles(directory,"*.sln").Count() > 0)
                    {
                        var sln = new SolutionMeta(directory);
                        

                        //Find .csproj file 
                        //Improve: parse this info from .sln file
                        var respr = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);
                        foreach (var proj in respr)
                        {
                            var project = new CSPROJ();
                            project.FileName = proj.Split('\\').Last();



                            var res = Directory.GetFiles( System.IO.Path.GetDirectoryName(proj), "*.cs", SearchOption.AllDirectories);
                            foreach (var re in res)
                            {
                                var cs = new CSFile();
                                cs.Path = re;
                                if (cs.Path.Contains("AssemblyInfo") ||
                                    cs.Path.Contains("TemporaryGeneratedFile"))
                                    continue;
                                cs.Content = File.ReadAllText(re);
                                project.CSFiles.Add(cs);
                            }
                            project.CSFiles =
                                project.CSFiles.OrderByDescending(p => p.FileName.ToLower().Contains("program.cs"))
                                       .ThenBy(p => p.FileName)
                                       .ToList();

                            sln.Csprojs.Add(project);

                        }
                        allslns.Add(sln);
                    }

                }

                lbSLNS.ItemsSource = allslns;
            }
        }

        private void LbSLNS_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbSLNS.SelectedIndex != -1)
            {
                lbPROJS.ItemsSource = (lbSLNS.SelectedItem as SolutionMeta).Csprojs;
                if (lbPROJS.Items.Count > 0)
                    lbPROJS.SelectedIndex = 0;
            }
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
                fileView.Text = (lbFilesInSLN.SelectedItem as CSFile).Content;
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
                        
                        
                        //Let's run this shit:
                        Process compiler = new Process();
                        compiler.StartInfo.FileName = "test.exe";
                        
                        compiler.StartInfo.UseShellExecute = false;
                        compiler.StartInfo.RedirectStandardOutput = true;

                        compiler.OutputDataReceived += new DataReceivedEventHandler(ProcessOuputHandler);
                        compiler.Start();
                        compiler.BeginOutputReadLine();
                        //tbRunoutput.Text =compiler.StandardOutput.ReadToEnd();
                        while (!compiler.HasExited)
                        {
                            // Refresh you're WPF window here
                            this.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
                            Thread.Sleep(1000);
                        }
                        
                        compiler.Close();

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

        public void ProcessOuputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                if (!lbRunoutput.Dispatcher.CheckAccess())
                {
                    // Called from a none ui thread, so use dispatcher
                    ShowLoggingDelegate showLoggingDelegate = new ShowLoggingDelegate(ShowLogging);
                    lbRunoutput.Dispatcher.Invoke(DispatcherPriority.Normal, showLoggingDelegate, outLine.Data);
                }
                else
                {
                    // Called from UI trhead so just update the textbox
                    ShowLogging(outLine.Data);
                };
            }
        }

        private delegate void ShowLoggingDelegate(string text);
        private static Action EmptyDelegate = delegate() { };

        /// <summary>
        /// Show the logging on screen
        /// </summary>
        /// <param name="text"></param>
        private void ShowLogging(string text)
        {
            lbRunoutput.Items.Add(text);

        }
    }


}
