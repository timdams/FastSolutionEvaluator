using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSolutionEvaluator.utility
{
    class ConsoleOutputAndLogger
    {
        private readonly string m_programPath;
        private readonly string m_logPath;
        private TextWriter m_writer;

        public ConsoleOutputAndLogger(string programPath, string logPath)
        {
            m_programPath = programPath;
            m_logPath = logPath;
        }

        public void Run()
        {
            using (m_writer = new StreamWriter(m_logPath))
            {

                var process =
                    new Process
                    {
                        StartInfo =
                            new ProcessStartInfo(m_programPath)
                            { RedirectStandardOutput = true, UseShellExecute = false }
                    };

                process.OutputDataReceived += OutputDataReceived;

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            m_writer.WriteLine(e.Data);
        }
    }
}
