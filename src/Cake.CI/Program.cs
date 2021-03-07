using Cake.Frosting;

namespace Cake.CI
{
    internal class Program
    {
        internal static int Main(string[] args)
        {
            return new CakeHost().UseContext<BuildContext>().Run(args);
        }
    }
}
