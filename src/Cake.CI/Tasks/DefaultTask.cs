using Cake.Frosting;

namespace Cake.CI.Tasks
{
    [IsDependentOn(typeof(SonarTask))]
    public sealed class Default : FrostingTask
    {
    }
}
