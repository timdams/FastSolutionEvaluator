using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSolutionLoader.Model
{
    public class SolutionModel
    {
        public SolutionModel(string path, Exception ex)//Used to create stub/Fake solution
        {
            PathToSln = path;
            Projects = new List<ProjectModel>();
            Solution = null;
            Exceptions.Add(ex);
        }

        public SolutionModel(SolutionFile file, string path)
        {
            PathToSln = path;
            Projects = new List<ProjectModel>();
            Solution = file;
            foreach (var proj in file.ProjectsInOrder)
            {
                try
                {
                    Projects.Add(new ProjectModel(proj.AbsolutePath));

                }
                catch (Exception ex)
                {

                    Debug.WriteLine(ex.Message);
                    Exceptions.Add(ex);
                    //TODO: create fake project??
                }

            }
        }
        public SolutionFile Solution { get; set; }
        public List<ProjectModel> Projects { get; set; } = new List<ProjectModel>();

        public string PathToSln { get; set; }

        public List<Exception> Exceptions { get; set; } = new List<Exception>();
    }
}
