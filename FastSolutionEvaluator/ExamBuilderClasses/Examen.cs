using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FastSolutionEvaluator.ExamBuilderClasses
{
    class Examen
    {
        public Examen()
        {
            Vragen = new List<Vraag>();
        }

        public int Jaar { get; set; }
        public string Titel { get; set; }
        public string Beschrijving { get; set; }

        public List<Vraag> Vragen { get; set; }

     
    }
}
