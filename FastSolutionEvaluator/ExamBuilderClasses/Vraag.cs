using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSolutionEvaluator.ExamBuilderClasses
{
    class Vraag
    {
        public string Titel { get; set; }
        public string Beschrijving { get; set; }
        public int Gewicht { get; set; }
        public VraagType VraagType { get; set; }
    }
}
