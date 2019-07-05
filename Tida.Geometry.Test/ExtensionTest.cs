using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tida.Geometry.External;
using Tida.Geometry.Primitives;

namespace Tida.Geometry.Test {
    [TestClass]
    public class ExtensionTest {
        [TestMethod]
        public void TestProjectOn() {
            var s = Extension.ProjectOn(Vector2D.BasisX + Vector2D.BasisY, new Line2D(Vector2D.Zero, Vector2D.BasisY + Vector2D.BasisX));
            var s2 = Extension.ProjectOn((Vector2D.BasisX + Vector2D.BasisY) * 2, new Line2D(Vector2D.Zero * 2, Vector2D.BasisY * 2));

            var s0 = Extension.ProjectOn(Vector2D.Zero, new Line2D(new Vector2D(1, 0), new Vector2D(1, 2)));
        }
    }
}

