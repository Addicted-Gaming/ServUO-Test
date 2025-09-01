using Server;

namespace Server.Tests;

public class InsensitiveTests
{
    [Test]
    public void Equals_IgnoresCase()
    {
        Assert.IsTrue(Insensitive.Equals("Hello", "hello"));
    }

    [Test]
    public void StartsWith_IgnoresCase()
    {
        Assert.IsTrue(Insensitive.StartsWith("ServUO", "serv"));
    }

    [Test]
    public void EndsWith_IgnoresCase()
    {
        Assert.IsTrue(Insensitive.EndsWith("ServUO", "uo"));
    }

    [Test]
    public void Contains_IgnoresCase()
    {
        Assert.IsTrue(Insensitive.Contains("The Quick Brown Fox", "quick"));
    }
}
