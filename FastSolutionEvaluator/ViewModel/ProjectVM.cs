using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSSolutionLoader.Model;

namespace FastSolutionEvaluator.ViewModel
{
    class ProjectVM
    {
        private ProjectModel proj;
        public ProjectModel Project { get { return proj; } }
        public ProjectVM(ProjectModel proj)
        {
            this.proj = proj;
            relevantFiles = new List<FileVM>();
            foreach (var file in proj.RelevantFiles)
            {
                relevantFiles.Add(new FileVM(file));
            }
           // string n = proj.Project.Properties.Where(p => p.Name == "MSBuildProjectName").FirstOrDefault().EvaluatedValue;
        }

        public string ProjectName
        {
            get
            {
                
                
                string n = proj.Project.Properties.Where(p => p.Name == "MSBuildProjectName").FirstOrDefault().EvaluatedValue;

                return n;
            }
        }

        public string BestExePath
        {
            get
            {

                string debugp = this.Project.Project.DirectoryPath + "\\bin\\debug\\" + this.ProjectName + ".exe";
                string releasep = this.Project.Project.DirectoryPath  + "\\bin\\release\\" + this.ProjectName + ".exe";
                string usethis = "null";
                if (File.Exists(debugp))
                    usethis = debugp;
                else if (File.Exists(releasep))
                    usethis = releasep;

                return usethis;
            }

        }


        private List<FileVM> relevantFiles;
        public List<FileVM> RelevantFiles
        {
           get { return relevantFiles; }
        }

       

    }
}
