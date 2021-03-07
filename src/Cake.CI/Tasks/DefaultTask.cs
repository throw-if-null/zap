using Cake.Frosting;

namespace Cake.CI.Tasks
{
    [IsDependentOn(typeof(TestTask))]
    public sealed class Default : FrostingTask
    {
    }
}
