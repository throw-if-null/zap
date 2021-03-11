using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.SonarScanner;
using System.Collections.Generic;

namespace Cake.CI.Tasks
{
    [TaskName("Sonar")]
    [IsDependentOn(typeof(TestTask))]
    public sealed class SonarTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Log.Information($"Working directory: {context.Environment.WorkingDirectory.FullPath}");

            var reportsExist = context.FileSystem.GetDirectory(context.Environment.WorkingDirectory.Combine("reports")).Exists;
            context.Log.Information($"Reports directory exists: {reportsExist}");

            var sourcesDirectory = context.FileSystem.GetDirectory(context.Environment.WorkingDirectory.FullPath.Remove(context.Environment.WorkingDirectory.FullPath.LastIndexOf('/')));

            context.Log.Information($"Source directory: {sourcesDirectory.Path.FullPath}");

            var settings = new SonarScannerSettings
            {
                Debug = true,
                Properties = new Dictionary<string, string>
                {
                    ["sonar.login"] = context.Environment.GetEnvironmentVariable("SONAR_TOKEN"),
                    ["sonar.host.url"] = "https://sonarcloud.io",
                    ["sonar.sources"] = sourcesDirectory.Path.FullPath,
                    ["sonar.projectBaseDir"] = "..",
                    ["sonar.organization"] = "mimme",
                    ["sonar.projectKey"] = "MirzaMerdovic_zap",
                    ["sonar.projectName"] = "zap",

                    ["sonar.inclusions"] = "**/*.cs",
                    ["sonar.cs.opencover.reportsPaths"] = "reports/*/coverage.opencover.xml",
                    ["sonar.coverage.exclusions"] = "**Test*.cs, **/bin/**, **/obj/**"
                }
            };

            context.SonarScanner(settings);
        }
    }
}
