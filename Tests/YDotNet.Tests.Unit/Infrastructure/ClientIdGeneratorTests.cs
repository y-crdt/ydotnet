using NUnit.Framework;
using YDotNet.Infrastructure;

namespace YDotNet.Tests.Unit.Infrastructure;

public class ClientIdGeneratorTests
{
    [Test]
    public void HasCorrectMaxValue()
    {
        Assert.That(ClientIdGenerator.MaxSafeInteger, Is.EqualTo((2 ^ 53) - 1));
    }
}
