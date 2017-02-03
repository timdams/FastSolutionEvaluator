using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Construction;
using Microsoft.Build.BuildEngine;

namespace VSSolutionLoader
{
    public class VSSLnLoader
    {
        static public List<SolutionModel> GetAllSolutions(string path, SearchOption searchoption = SearchOption.AllDirectories)
        {
            //This class is found in the Microsoft.Build.dll 14.0.0.0 assembly. In my case it was located at:
            //         C:\Program Files (x86)\Reference Assemblies\Microsoft\MSBuild\v14.0\Microsoft.Build.dll
            //Source: http://stackoverflow.com/questions/707107/parsing-visual-studio-solution-files
            var allslns = new List<SolutionModel>();
            foreach (var directory in Directory.GetDirectories(path, "*.*", searchoption))
            {
                var slns = Directory.GetFiles(directory, "*.sln");
                foreach (var item in slns)
                {
                    var sol = SolutionFile.Parse(item);
                    var somodel = new SolutionModel( sol );
                    

                    allslns.Add(somodel);

                }


            }
            return allslns;
        }


    }

    public class SolutionModel
    {
        public SolutionModel(SolutionFile file)
        {
            Projects = new List<ProjectModel>();
            Solution = file;
            foreach (var proj in file.ProjectsInOrder)
            {
                 
                Projects.Add(new ProjectModel(proj.AbsolutePath));
            }
        }
        public SolutionFile Solution { get; set; }
        public List<ProjectModel> Projects { get; set; }
    }

    public class ProjectModel
    {
        public ProjectModel(string path)
        {
            Project= new Microsoft.Build.Evaluation.Project(path, null, "14.0");
        }
        public Microsoft.Build.Evaluation.Project Project { get; set; }
    }
}