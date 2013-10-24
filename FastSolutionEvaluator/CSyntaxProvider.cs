using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace FastSolutionEvaluator
{
    public class CSyntaxProvider //Source: http://blogs.microsoft.co.il/blogs/tamir/archive/2006/12/14/RichTextBox-syntax-highlighting.aspx
    {
        private static List<string> tags = new List<string>();
        private static List<char> specials = new List<char>();

        static CSyntaxProvider()
        {
            string[] strs =
                {
                   "abstract","event","new","struct","as","explicit","null","switch","base","extern","object","this","bool","false","operator","throw","break","finally","out","true","byte","fixed","override","try","case",
                   "float","params","typeof","catch","for","private","uint","char","foreach","protected","ulong","checked","goto","public","unchecked","class","if","readonly","unsafe","const","implicit","ref","ushort",
                   "continue","in","return","using","decimal","int","sbyte","virtual","default","interface","sealed","volatile","delegate","internal","short","void","do","is","sizeof","while","double","lock",
                   "stackalloc","else","long","static","enum","namespace","string"
                };
            tags = new List<string>(strs);
            char[] chrs =
                {
                    '.',
                    ')',
                    '(',
                    '[',
                    ']',
                    '>',
                    '<',
                    ':',
                    ';',
                    '\n',
                    '\t'
                };
            specials = new List<char>(chrs);
        }

        public static List<char> GetSpecials
        {
            get { return specials; }
        }
        public static List<string> GetTags
        {
            get { return tags; }
        }
        public static bool IsKnownTag(string tag)
        {
            return tags.Exists(delegate(string s) { return s.ToLower().Equals(tag.ToLower()); });
        }
        public static List<string> GetJSProvider(string tag)
        {
            return tags.FindAll(delegate(string s) { return s.ToLower().StartsWith(tag.ToLower()); });
        }
    }

    struct Tag
    {
        public TextPointer StartPosition;
        public TextPointer EndPosition;
        public string Word;

    }
}
