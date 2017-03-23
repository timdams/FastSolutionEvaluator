using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VSSolutionLoader.Model;

namespace FastSolutionEvaluator.ViewModel
{
    class SolutionVM : BaseVM
    {

        //http://stackoverflow.com/questions/36149863/how-to-write-viewmodelbase-in-mvvm-wpf
        private SolutionModel solution;

        public SolutionVM(SolutionModel item)
        {
            this.solution = item;
            projects = new List<ProjectVM>();
            foreach (var proj in solution.Projects)
            {

                var p = new ProjectVM(proj);//If this line gives a strange MethodMissingException, make sure you load the 14.0 MSBuild assemblies and not the buildin ones!
                projects.Add(p);
            }
        }

        public string SolutionName
        {
            get
            {
                if(solution.Exceptions.Count==0)
                    return Path.GetFileName(solution.PathToSln).Split('.').First();
                return solution.PathToSln;
               
            }
            // set { SetProperty(ref _firstName, value); }
        }


        private List<ProjectVM> projects;
        public List<ProjectVM> Projects
        {
            get { return projects; }
        }

        public string PathToSln
        {
            get { return solution.PathToSln; }
        }
        public Visibility HasExceptions
        {
            get
            {
                if (solution.Exceptions.Count > 0)
                    return Visibility.Visible;
                return Visibility.Hidden;
            }


        }

        public Visibility HasNoExeVisibile
        {
            get
            {
                if (Projects.Count > 0)
                {
                    foreach (var item in Projects)
                    {
                        if (item.BestExePath != "null")
                        {
                            return Visibility.Hidden;
                        }

                    }

                }
                return Visibility.Visible;
            }
        }

        private string friendlyname = "";
        public string FriendlyName
        {
            get
            {
                if (friendlyname == "") return this.SolutionName;
                return friendlyname;
            }
            set
            {
                friendlyname = value;//TODO: gebruiker via rechterklik dit laten instellen
            }
        }
    }

}
