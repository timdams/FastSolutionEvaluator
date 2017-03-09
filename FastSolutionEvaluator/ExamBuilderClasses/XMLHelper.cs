using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FastSolutionEvaluator.ExamBuilderClasses
{
    class XMLHelper
    {
        //https://codevan1001nacht.wordpress.com/2014/01/17/werken-met-xml-bestanden/
        public static void SaveToXml<T>(T tosave, string filepath)
        {
            var x = new XmlSerializer(tosave.GetType());
            using (var writer = System.Xml.XmlWriter.Create(filepath))
            {
                x.Serialize(writer, tosave);
            }
        }


        public static T LoadFromXml<T>(string filepath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = System.Xml.XmlReader.Create(filepath))
            {
                return (T)serializer.Deserialize(reader);
            }

        }
    }
}
