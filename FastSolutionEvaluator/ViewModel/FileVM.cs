using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSSolutionLoader.Model;

namespace FastSolutionEvaluator.ViewModel
{
    class FileVM : BaseVM
    {
        private FileModel file;

        public FileVM(FileModel file)
        {
            this.file = file;
        }

        public string Content
        {
            get { return file.Content; }
        }

        public ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition ViewerType
        {
            get
            {
                return utility.AvalonHelper.GetCorrectSyntaxHighligther(System.IO.Path.GetExtension(Path));
            }
        }

        public string Name
        {
            get { return file.FileName; }
        }

        public string Path
        {
            get { return file.FullPath; }
        }
    }
}
