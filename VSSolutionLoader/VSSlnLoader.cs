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
using VSSolutionLoader.Model;

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
                if (!directory.Contains(".git"))
                {
                    var slns = Directory.GetFiles(directory, "*.sln");
                    foreach (var item in slns)
                    {
                        try
                        {
                            var sol = SolutionFile.Parse(item);// http://answers.flyppdevportal.com/MVC/Post/Thread/c377dae1-fe1e-4635-a284-f17b3d580b69?category=msbuild
                            var somodel = new SolutionModel(sol, item);


                            allslns.Add(somodel);
                        }
                        catch (Exception ex)
                        {

                            Debug.WriteLine($"Couldn't open {item}.Error: {ex.Message}. I created a stub solution");
                            var somodel = new SolutionModel( item, ex);
                            allslns.Add(somodel);


                        }
                       
                       

                    }
                }


            }
            return allslns;
        }


    }





   
}