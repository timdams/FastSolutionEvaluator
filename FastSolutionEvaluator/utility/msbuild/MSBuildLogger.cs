
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace FastSolutionEvaluator.utility.msbuild
{
    class MSBuildLogger : Logger
    {
        private StringBuilder errorLog = new StringBuilder();

        public string BuildErrors { get; private set; }

        /// <summary>
        /// This will gather info about the projects built
        /// </summary>
        public IList<string> BuildDetails { get; private set; }

        /// <summary>
        /// Initialize is guaranteed to be called by MSBuild at the start of the build
        /// before any events are raised.
        /// </summary>
        public override void Initialize(IEventSource eventSource)
        {
            BuildDetails = new List<string>();

            // FOR BREVITY, WE'LL ONLY REGISTER FOR CERTAIN EVENT TYPES.
            eventSource.ProjectStarted += new ProjectStartedEventHandler(eventSource_ProjectStarted);
            eventSource.ErrorRaised += new BuildErrorEventHandler(eventSource_ErrorRaised);

        }

        void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            // BUILDERROREVENTARGS ADDS LINENUMBER, COLUMNNUMBER, FILE, AMONGST OTHER PARAMETERS
            string line = String.Format(": ERROR {0}({1},{2}): ", e.File, e.LineNumber, e.ColumnNumber);
            errorLog.Append(line + e.Message);
        }

        void eventSource_ProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            BuildDetails.Add(e.Message);
        }

        /// <summary>
        /// Shutdown() is guaranteed to be called by MSBuild at the end of the build, after all
        /// events have been raised.
        /// </summary>
        public override void Shutdown()
        {
            // DONE LOGGING, LET GO OF THE FILE
            BuildErrors = errorLog.ToString();

        }
    }
}
