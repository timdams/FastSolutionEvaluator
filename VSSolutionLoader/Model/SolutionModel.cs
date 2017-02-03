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
        public SolutionModel(SolutionFile file)
        {
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
                }

            }
        }
        public SolutionFile Solution { get; set; }
        public List<ProjectModel> Projects { get; set; }
    }
}
