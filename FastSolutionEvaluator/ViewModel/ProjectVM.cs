using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSSolutionLoader.Model;

namespace FastSolutionEvaluator.ViewModel
{
    class ProjectVM
    {
        private ProjectModel proj;

        public ProjectVM(ProjectModel proj)
        {
            this.proj = proj;
            relevantFiles = new List<FileVM>();
            foreach (var file in proj.RelevantFiles)
            {
                relevantFiles.Add(new FileVM(file));
            }
        }

        public string ProjectName
        {
            get
            {
                return proj.Project.Properties.Where(p => p.Name == "MSBuildProjectName").FirstOrDefault().EvaluatedValue;
            }
        }


        private List<FileVM> relevantFiles;
        public List<FileVM> RelevantFiles
        {
           get { return relevantFiles; }
        }

    }
}
