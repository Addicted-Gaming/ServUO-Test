using Server;
using Xunit;

namespace Server.Tests;

public class UtilityTests {
    [Theory]
    [InlineData("127.0.0.1", true)]
    [InlineData("not.an.ip", false)]
    public void IsValidIP_ReturnsExpected(string ip, bool expected) {
        var result = Utility.IsValidIP(ip);
        Assert.Equal(expected, result);
    }
}
