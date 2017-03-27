using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSolutionEvaluator.utility
{
    class AvalonHelper
    {
        public static ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition GetCorrectSyntaxHighligther(string extension)
        {
            //https://github.com/icsharpcode/AvalonEdit/blob/master/ICSharpCode.AvalonEdit/Highlighting/Resources/Resources.cs
            // http://midnightprogrammer.net/post/Syntax-Highlighter-In-WPF/

    
            //WOE to however enters this code and tries to use it. Just stay of it..It's buttugly, shouldn't exist, yet it works.. A bit like the average political party in the 21th century

            if (extension.StartsWith(".")) extension = extension.Split('.').Last();

            string res = "notfound";
            if ((res = GetHighlight("XML", new[] { ".xml", ".xaml", "axml" }, extension)) == "notfound")
                if ((res = GetHighlight("C#", new[] { ".cs" }, extension)) == "notfound")
                    if ((res = GetHighlight("JavaScript", new[] { ".boo" }, extension)) == "notfound")
                        if ((res = GetHighlight("HTML", new[] { ".htm", ".html" }, extension)) == "notfound")
                            if ((res = GetHighlight("ASP/XHTML", new[] { "asp", "aspx", "asax", "asmx", "ascx", "master" }, extension)) == "notfound")
                                if ((res = GetHighlight("Boo", new[] { ".boo" }, extension)) == "notfound")
                                    if ((res = GetHighlight("Coco", new[] { ".atg" }, extension)) == "notfound")
                                        if ((res = GetHighlight("CSS", new[] { ".css" }, extension)) == "notfound")
                                            if ((res = GetHighlight("C++", new[] { ".c", ".h", ".cc", ".cpp", ".hpp" }, extension)) == "notfound")
                                                if ((res = GetHighlight("Java", new[] { ".java" }, extension)) == "notfound")
                                                    if ((res = GetHighlight("Patch", new[] { ".patch", ".diff" }, extension)) == "notfound")
                                                        if ((res = GetHighlight("PowerShell", new[] { ".ps1", ".psm1", ".psd1" }, extension)) == "notfound")
                                                            if ((res = GetHighlight("PHP", new[] { ".php" }, extension)) == "notfound")
                                                                if ((res = GetHighlight("TeX", new[] { ".tex" }, extension)) == "notfound")
                                                                    if ((res = GetHighlight("VB", new[] { ".vb" }, extension)) == "notfound")
                                                                        if ((res = GetHighlight("XML", (".xml;.xsl;.xslt;.xsd;.manifest;.config;.addin;" +
                                                             ".xshd;.wxs;.wxi;.wxl;.proj;.csproj;.vbproj;.ilproj;" +
                                                             ".booproj;.build;.xfrm;.targets;.xaml;.xpt;" +
                                                             ".xft;.map;.wsdl;.disco;.ps1xml;.nuspec").Split(';')
                                                    , extension)) == "notfound")
                                                                            if ((res = GetHighlight("MarkDown", new[] { ".md" }, extension)) == "notfound")
                                                                                res = "notfound";

            return ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition(res);
        }

        private static string GetHighlight(string v1, string[] v2, string extension)
        {
            foreach (var ext in v2)
            {
                if (ext.EndsWith(extension)) return v1;
            }
            return "notfound";
        }
    }
}
