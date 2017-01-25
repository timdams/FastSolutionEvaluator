using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FastSolutionEvaluator
{
    class SolutionMeta
    {

        public string FullPath { get; set; }

        public string FolderName { get { return Path.GetFileName(FullPath); } }

        public override string ToString()
        {
            return FolderName;
        }


        public Visibility HasNoExeVisibile
        {
            get
            {
                if (Csprojs.Count > 0)
                {

                    if (BestExePath != "null")
                    {
                        return Visibility.Hidden;
                    }
                }
                return Visibility.Visible;
            }
        }

        public Visibility HasGradeVisibile
        {
            get
            {
                if (IsEvaled)
                    return Visibility.Visible;
                return Visibility.Hidden;

            }
        }


        public string BestExePath
        {
            get
            {
                string debugp = FullPath + "\\" + Csprojs[0].ToString() + "\\bin\\debug\\" + Csprojs[0].ToString() + ".exe";
                string releasep = FullPath + "\\" + Csprojs[0].ToString() + "\\bin\\release\\" + Csprojs[0].ToString() + ".exe";
                string noproj = FullPath + "\\bin\\debug\\" + Csprojs[0].ToString() + ".exe";
                string usethis = "null";
                if (File.Exists(debugp))
                    usethis = debugp;
                else if (File.Exists(releasep))
                    usethis = releasep;
                else if (File.Exists(noproj))
                    usethis = noproj;
                return usethis;
            }

        }


        public List<CSPROJ> Csprojs { get; set; }
        public bool IsEvaled { get; internal set; }

        public SolutionMeta(string path)
        {
            FullPath = path;
            Csprojs = new List<CSPROJ>();
        }



    }

    class CSPROJ
    {
        public string FileName { get; set; }
        public List<CSFile> CSFiles { get; set; }
        public CSPROJ()
        {
            CSFiles = new List<CSFile>();
        }

        public override string ToString()
        {
            return FileName.Split('\\').Last().Replace(".csproj", "");
        }
    }
    class CSFile
    {
        public string FileName { get { return System.IO.Path.GetFileName(Path); } }
        public string Content { get; set; }

        public override string ToString()
        {
            return FileName;
        }

        public string Path { get; set; }
    }
}
