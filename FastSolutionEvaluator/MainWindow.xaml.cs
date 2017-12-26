using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FastSolutionEvaluator.ViewModel;
using FastSolutionEvaluator.ExamBuilderClasses;

namespace FastSolutionEvaluator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string evalfilepath = Properties.Settings.Default.LastPath + "\\evalsresults.csv";
        public Examen GekoppeldExamen { get; set; }
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }
        List<SolutionVM> allslns = null;
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            dialog.SelectedPath = Properties.Settings.Default.LastPath;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!File.Exists(dialog.SelectedPath + "\\koppel.xml"))
                {
                    var wnd = new KoppelExamenAanFolderWindow();
                    wnd.ShowDialog();

                    if (wnd.TeKoppelenExamenPath != "")
                    {

                        KoppelExamen k = new KoppelExamen();
                        k.ExamFilePath = wnd.TeKoppelenExamenPath;
                        k.FolderPath = dialog.SelectedPath;
                        XMLHelper.SaveToXml<KoppelExamen>(k, dialog.SelectedPath + "\\koppel.xml");
                        //TODO: best file mee in folder proppen? Misschien aanbevelen aan gebruiker om dit te doen
                        var koppel = XMLHelper.LoadFromXml<KoppelExamen>(dialog.SelectedPath + "\\koppel.xml");
                        GekoppeldExamen = XMLHelper.LoadFromXml<Examen>(koppel.ExamFilePath);
                        //TODO: vragen UI genereren
                        GenerateExamVragenUI();
                    }
                }
                else
                {
                    var koppel = XMLHelper.LoadFromXml<KoppelExamen>(dialog.SelectedPath + "\\koppel.xml");
                    GekoppeldExamen = XMLHelper.LoadFromXml<Examen>(koppel.ExamFilePath);
                    //TODO: vragen UI genereren
                    GenerateExamVragenUI();
                }


                Properties.Settings.Default.LastPath = dialog.SelectedPath;
                Properties.Settings.Default.Save();

              allslns  = SolutionsVM.Load(dialog.SelectedPath).OrderBy(p=>p.SolutionName).ToList();

                lbSLNS.ItemsSource = allslns;



                //TODO: implement in new eval datamode
                if (File.Exists(evalfilepath))
                {  //Update eval status
                    string logfile = File.ReadAllText(evalfilepath);
                    foreach (var solution in allslns)
                    {
                        if (GetLineNumber(logfile, solution.PathToSln) > -1)
                            solution.IsEvaled = true;
                    }
                }
                UpdateCountdown();
            }
            //Show it




        }

        private void btnOpenExamBuilder_Click(object sender, RoutedEventArgs e)
        {
            ExamBuilderWindow wnd = new ExamBuilderWindow();
            wnd.ShowDialog();
        }
        private void GenerateExamVragenUI()
        {
            int teller = 1;
            string huidigeCategorie = "BLIBLIABA";
            ExamVragenLijstUI.Items.Clear();
            foreach (var vraag in GekoppeldExamen.Vragen)
            {
                //               <TextBlock FontWeight="Bold">Inleiding UI (1p)</TextBlock>
                //     < CheckBox Name = "chkbUI" Checked = "chkbUI_Click" Unchecked = "chkbUI_Click" > Hoofdmodule en resetknop IsEnabled, rest niet</ CheckBox >

                //            < CheckBox Name = "chkbUI2" Checked = "chkbUI_Click" Unchecked = "chkbUI_Click" > Gebruikt groupboxes </ CheckBox >

                if (huidigeCategorie != vraag.Titel)
                {
                    TextBlock txbtitel = new TextBlock();
                    txbtitel.FontWeight = FontWeights.Bold;
                    txbtitel.Text = vraag.Titel;
                    ExamVragenLijstUI.Items.Add(txbtitel);
                    huidigeCategorie = vraag.Titel;
                }

                Control contr = new Control();

                switch (vraag.VraagType)
                {
                    case VraagType.TrueFalse:
                        CheckBox cb = new CheckBox();
                        cb.Content = $"{teller}:{vraag.Beschrijving} ({vraag.Gewicht} p)";
                        cb.DataContext = vraag;
                        contr = cb;
                        break;
                    case VraagType.Tekst:
                        TextBox tb = new TextBox();
                        tb.Text = $"{teller}:{vraag.Beschrijving} ({vraag.Gewicht} p)";
                        contr = tb;
                        break;
                    case VraagType.Slider:
                        var sl = new TextBox();
                        sl.Text = $"{teller}:SLIDER: {vraag.Beschrijving} ({vraag.Gewicht} p)[NOG NIET WERKENDE]";
                        contr = sl;
                        break;
                    case VraagType.GeheelGetal:
                        var gh = new TextBox();
                        gh.Text = $"{teller}:GEHEELGETAL: {vraag.Beschrijving} ({vraag.Gewicht} p)[NOG NIET WERKENDE]";
                        contr = gh;
                        break;
                }
                ExamVragenLijstUI.Items.Add(contr);
                teller++;
            }

            write = new Button();
            write.HorizontalAlignment = HorizontalAlignment.Stretch;
            write.Content = "Schrijf weg";
            write.MinWidth = 100;
            write.Click += WriteResults_Click;
            ExamVragenLijstUI.Items.Add(write);
        }
        Button write;
        private void ClearExamUIControls()
        {
            //Todo: bewaren?

            foreach (var item in ExamVragenLijstUI.Items)
            {
                if (item is CheckBox) (item as CheckBox).IsChecked = false;
                else if (item is TextBox) (item as TextBox).Text = "";
            }
        }
        private void WriteResults_Click(object sender, RoutedEventArgs e)
        {
            //TODO: keep this in sync with  reading results from the csv!

            if (lbSLNS.SelectedItem != null) //TODO: deze knop disablen
            {
                string res = "";
                var sol = lbSLNS.SelectedItem as SolutionVM;
                res += $"{sol.FriendlyName};";


                foreach (var contr in ExamVragenLijstUI.Items)
                {
                    if (contr is Control)
                    {
                        if (contr is CheckBox)
                        {
                            //TODO: less fucked up code.Databinding+ datacontext gebruiken
                            CheckBox chk = contr as CheckBox;
                            if (chk.IsChecked == true) res += $"{(chk.DataContext as Vraag).Gewicht};";
                            else res += "0;";
                        }
                        //TODO: andere controls wegschrijven

                    }
                }
                res += $"{sol.PathToSln};{DateTime.Now}";
                //MessageBox.Show(res);
                //WriteResultLine(res, evalfilepath, sol);
                WriteEvalStuffToFile(res, sol);
            }
        }
        #region main actions on top menu
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
                            //TODO: if( only with console)
                            if (selsol.ProjectOutputType == OutputType.Console)
                            {
                                ConsoleWindow wnd = new ConsoleWindow();
                                wnd.PathToProc = selsol.BestExePath;
                                wnd.Show();
                            }
                            else
                            {
                             
                                 var Proc = Process.Start(selsol.BestExePath);
                            }

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

            //TODO: programma crasht indien \temp niet bestaat
            if (lbSLNS.SelectedIndex != -1)
            {
                try
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
                        //TODO: nieuwe build vinden want oa $ notatie faalt
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
                            var files = Directory.GetFiles(temppath);
                            foreach (var file in files)
                            {
                                if (file.EndsWith("exe"))
                                    Process.Start(file);//TODO: console screen will dissapear immediately
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("FAIL:" + sb.ToString().Split(new string[] { "Build FAILED" }, StringSplitOptions.None).Last(), "FAILURE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Big fail" + ex.Message) ;
                }
            }
        }
        #endregion


        #region write exam eval stuff to file
        private void WriteEvalStuffToFile(string result, SolutionVM SolutionName)
        {
            //TODO: sanitize stuff so no ';' are inside the stuff to write away

            if (File.Exists(evalfilepath))
            {
                //TODO: write stuff away
                int line = GetLineNumber(File.ReadAllText(evalfilepath), SolutionName.PathToSln);
                if (line != -1)
                {
                    if (MessageBox.Show("bestaat al... now what? Overwrite?", "Oei", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        File_DeleteLine(line, evalfilepath);
                        WriteResultLine(result, evalfilepath, SolutionName);

                    }
                }
                else{
                    WriteResultLine(result, evalfilepath, SolutionName);
                }
            }
            else
            {
                WriteResultLine(result, evalfilepath, SolutionName);
            }
        }

        private  void WriteResultLine(string result, string evalfilepath, SolutionVM sol)
        {
            var f = File.AppendText(evalfilepath);
            f.WriteLine(result);
            f.Close();
            sol.IsEvaled = true;

            //UPdate status
            this.UpdateCountdown();

        }

        private  void UpdateCountdown()
        {
            int total = allslns.Count;
            int evaled = allslns.Where(p => p.IsEvaled).Count();

            txbStatus.Text = $"{evaled}/{total}";
        }

        string File_ReturnLine(string Line, string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                int Countup = 0;
                while (!sr.EndOfStream)
                {
                    Countup++;
                    string line = sr.ReadLine();
                    if (line.Contains(Line))
                    {
                        return line;
                    }
                }
            }
            return "";
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
                    if (line.ToLower().Contains(lineToFind.ToLower()))
                        return lineNum;
                }
            }
            return -1;
        }
        #endregion

        private void lbFilesInProj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbFilesInProj.SelectedIndex != -1)
            {
                //AvalaonEdit not very bindable :/
                fileView.Load((lbFilesInProj.SelectedItem as FileVM).Path);
                fileView.SyntaxHighlighting = (lbFilesInProj.SelectedItem as FileVM).ViewerType;
            }
        }

        #region rightclick context menu
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
        #endregion
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    OpenSolutionInVS_Click(this, null);
                    break;

            }
        }


        private void lbSLNS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbSLNS.SelectedIndex > -1)
            {
                lbFilesInProj.SelectedIndex = -1;
                lbPROJS.SelectedIndex = -1;
                fileView.Clear();
                //Clear ExamUI stuff 
                ClearExamUIControls();

                //Load scores?
                TryLoadScores(lbSLNS.SelectedItem as SolutionVM);
            }
        }

        private void TryLoadScores(SolutionVM solutionVM)
        {
            if (File.Exists(evalfilepath))
            {
                string line = File_ReturnLine(solutionVM.PathToSln, evalfilepath);
                if (line != "")
                {
                    //MessageBox.Show(line);
                    //Get the results
                    var csvsplit = line.Split(';');
                    int count = 0;
                    foreach (var item in ExamVragenLijstUI.Items)
                    {
                        //TODO: complete me!
                        if (item is Control)
                        {
                            if (item is CheckBox)
                            {
                                if (csvsplit[count+1] != "0")
                                    (item as CheckBox).IsChecked = true;
                            }
                            count++;
                        }
                    }

                }

            }

        }

        private void btnShowGitK_Click(object sender, RoutedEventArgs e)
        {
            if (lbSLNS.SelectedIndex != -1)
            {

                //MessageBox.Show((lbSLNS.SelectedItem as SolutionMeta).FullPath+"\\"+lbPROJS.SelectedItem + "\\bin\\debug\\"+lbPROJS.SelectedItem+".exe");
                //
                string debugp = (lbSLNS.SelectedItem as SolutionVM).PathToSln;
                try
                {
                   var path= System.IO.Path.GetDirectoryName(debugp);
                  
                    var psi = new ProcessStartInfo("gitk");
                    psi.WorkingDirectory = path;
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    lbLog.Items.Insert(0, ex.Message);
                }
            }
        }
    }
}





