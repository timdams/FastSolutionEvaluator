using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FastSolutionEvaluator.ExamBuilderClasses
{
    public class Examen
    {
        public Examen()
        {
            Vragen = new ObservableCollection<Vraag>();
        }

        public int Jaar { get; set; }
        public string Titel { get; set; }
        public string Beschrijving { get; set; }

        public ObservableCollection<Vraag> Vragen { get; set; }

     
    }
}
