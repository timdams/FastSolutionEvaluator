using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSolutionLoader.Model
{
    public class ProjectModel
    {
        public ProjectModel(string path)
        {
            Project = new Microsoft.Build.Evaluation.Project(path, null, "4.0");
        }
        public Microsoft.Build.Evaluation.Project Project { get; set; }


        public List<FileModel> RelevantFiles
        {
            get
            {
                var res = new List<FileModel>();
                //TODO: filter instellen
                var rel = Project.Items.Where(
                    p => (p.ItemType == "Compile" && (p.EvaluatedInclude.EndsWith("cs"))&& !p.EvaluatedInclude.Contains("Designer.cs") && !p.EvaluatedInclude.Contains("AssemblyInfo"))
                    || p.ItemType=="Page"
                    );
                
                foreach (var file in rel)
                {
                    FileModel m = new FileModel(file.Project.DirectoryPath + "\\" + file.EvaluatedInclude);

                    res.Add(m);
                }
                return res;
            }
        }

    }
}
