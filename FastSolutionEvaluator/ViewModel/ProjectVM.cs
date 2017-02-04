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
        }

        public string ProjectName
        {
            get
            {
                return proj.Project.Properties.Where(p => p.Name == "MSBuildProjectName").FirstOrDefault().EvaluatedValue;
            }
        }
    }
}
