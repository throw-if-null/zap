using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core.IO;
using Cake.Coverlet;
using Cake.Frosting;
using System;

namespace Cake.CI.Tasks
{
    [TaskName("Test")]
    [IsDependentOn(typeof(BuildTask))]
    public sealed class TestTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            var coverletSettings = new CoverletSettings
            {
                CollectCoverage = true,
                CoverletOutputFormat = CoverletOutputFormat.opencover,
                CoverletOutputDirectory = context.Environment.WorkingDirectory.Combine(new DirectoryPath("reports")),
                CoverletOutputName = $"results-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}",
                Verbosity = DotNetCoreVerbosity.Diagnostic,
                DiagnosticOutput = true
            };

            context.DotNetCoreTest(
                "../Monitor.sln",
                new DotNetCoreTestSettings
                {
                    Configuration = context.MsBuildConfiguration,
                    NoBuild = true,
                    DiagnosticOutput = true,
                    Verbosity = DotNetCoreVerbosity.Normal
                },
                coverletSettings);
        }
    }
}
