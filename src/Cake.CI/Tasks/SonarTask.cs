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

            var reportsExist = context.FileSystem.GetDirectory(context.Environment.WorkingDirectory.Combine("reports"));
            context.Log.Information($"Reportes directory exists: {reportsExist}");


            var settings = new SonarScannerSettings
            {
                Properties = new Dictionary<string, string>
                {
                    ["sonar.login"] = "2ab9d8d0e8fde1b29cec17186a3e8c4df76e1cd6" //context.Environment.GetEnvironmentVariable("sonar_scanner_token")
                    //["sonar.login"] = "d864617b60288b7232fd9f821d959b63e7752b37"
                }
            };

            context.SonarScanner(settings);
        }
    }
}
