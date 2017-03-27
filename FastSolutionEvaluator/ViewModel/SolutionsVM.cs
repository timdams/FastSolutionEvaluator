using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSolutionEvaluator.ViewModel
{
    class SolutionsVM
    {
        internal static List<SolutionVM> Load(string selectedPath)
        {
            var sols= VSSolutionLoader.VSSLnLoader.GetAllSolutions(selectedPath);
            List<SolutionVM> res = new List<SolutionVM>();
            foreach (var item in sols)
            {
                res.Add(new SolutionVM(item));
            }
            return res;
        }
    }
}
