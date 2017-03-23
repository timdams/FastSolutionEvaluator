using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSolutionEvaluator.ExamBuilderClasses
{
  public  class Vraag
    {
        public string Titel { get; set; } = "leeg";
        public string Beschrijving { get; set; } = "leeg";
        public int Gewicht { get; set; } = 1;
        public VraagType VraagType { get; set; } = VraagType.TrueFalse;

        
    }
}
