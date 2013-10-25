using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<CSPROJ> Csprojs { get; set; }

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
            return FileName.Split('\\').Last().Replace(".csproj","");
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
