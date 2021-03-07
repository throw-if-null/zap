using Cake.Common.IO;
using Cake.Frosting;

namespace Cake.CI.Tasks
{
    [TaskName("Clean")]
    public sealed class CleanTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.CleanDirectories("../**/bin", x => !x.Path.FullPath.Contains("Cake.CI"));
        }
    }
}
