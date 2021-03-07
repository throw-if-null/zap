using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Frosting;

namespace Cake.CI.Tasks
{
    [TaskName("Build")]
    [IsDependentOn(typeof(CleanTask))]
    public sealed class BuildTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetCoreBuild("../Monitor.sln", new DotNetCoreBuildSettings
            {
                Configuration = context.MsBuildConfiguration
            });
        }
    }
}
