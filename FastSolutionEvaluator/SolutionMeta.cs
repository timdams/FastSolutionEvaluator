using System.Collections.Generic;

namespace FastSolutionEvaluator
{
    class SolutionMeta
    {
        public string FolderName { get; set; }

        public override string ToString()
        {
            return FolderName;
        }

        public List<CSFile> CSFiles { get; set; }


        public SolutionMeta()
        {
            CSFiles = new List<CSFile>();
        }
    }
}