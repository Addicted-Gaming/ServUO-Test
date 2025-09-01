using NUnit.Framework;
using Server;

namespace Server.Tests;

public class UtilityTests
{
    [Test]
    public void InRange_ReturnsTrue_WhenPointsWithinRange()
    {
        var p1 = new Point3D(0, 0, 0);
        var p2 = new Point3D(1, 1, 0);

        Assert.IsTrue(Utility.InRange(p1, p2, 2));
    }

    [Test]
    public void InRange_ReturnsFalse_WhenPointsOutsideRange()
    {
        var p1 = new Point3D(0, 0, 0);
        var p2 = new Point3D(5, 0, 0);

        Assert.IsFalse(Utility.InRange(p1, p2, 2));
    }
}
