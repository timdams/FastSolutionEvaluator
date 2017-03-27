using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSolutionLoader.Model
{
    public class FileModel
    {
        public FileModel(string path)
        {
            FullPath = path;
        }
        public string FileName { get { return System.IO.Path.GetFileName(FullPath); } }

        private string content;
        public string Content
        {
            get
            {
                if (content == null)
                {
                    try
                    {
                        content = File.ReadAllText(FullPath);
                    }
                    catch (Exception ex)
                    {

                        content = $"File unreadable ({ex.Message})";
                    }

                }
                return content;
            }
        }

        public override string ToString()
        {
            return FileName;
        }

        public string FullPath { get; private set; }
    }
}
