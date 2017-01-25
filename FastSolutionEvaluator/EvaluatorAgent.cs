using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FastSolutionEvaluator
{
    class EvaluatorAgent
    {
        //Achievementscode (i.e. codeagntic stuff) should be awesome here...For another time
        internal static void CheckThis(CSPROJ cSPROJ)
        {
            //Vind BerekenTotaalPrijs
            foreach (CSFile file in cSPROJ.CSFiles)
            {
                int pos = file.Content.ToLower().IndexOf("berekentotaalprijs(");
                if(pos!=-1)
                {
                    MessageBox.Show(file.Content.Substring(pos));
                }
            }
        }
    }
}
