using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\Dropbox\Lessen\Examen\1617\RESULTATEN\ProgPrincMod1_Poging2";

            var res= VSSolutionLoader.VSSLnLoader.GetAllSolutions(path);

        }
    }
}
