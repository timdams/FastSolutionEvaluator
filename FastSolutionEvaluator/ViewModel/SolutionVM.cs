using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSSolutionLoader.Model;

namespace FastSolutionEvaluator.ViewModel
{
    class SolutionVM:BaseVM
    {

        //http://stackoverflow.com/questions/36149863/how-to-write-viewmodelbase-in-mvvm-wpf
        private SolutionModel solution;

        public SolutionVM(SolutionModel item)
        {
            this.solution = item;
            projects = new List<ProjectVM>();
            foreach (var proj in solution.Projects)
            {
                projects.Add(new ProjectVM(proj));
            }
        }

        public string SolutionName
        {
            get {

                return Path.GetFileName(solution.PathToSln).Split('.').First();
            }
           // set { SetProperty(ref _firstName, value); }
        }


        private List<ProjectVM> projects;
        public List<ProjectVM> Projects
        {
          get { return projects; }
        }
    }
}
