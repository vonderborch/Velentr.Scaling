using System.Drawing;

namespace Velentr.Scaling.dev
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // insert test code here
            var parent = new Scalar(0, 0, 1920, 1080);
            var child = new Scalar(32, 32, 1024, 1024, parent);
            var child2 = new Scalar(32, 32, 1920, 1080, child);

            var coords = new Point(4, 4);
        }
    }
}
