using NUnit.Framework;
using YDotNet.Infrastructure;

namespace YDotNet.Tests.Unit.Infrastructure;

public class ClientIdTests
{
    [Test]
    public void TestSafety()
    {
        for (var i = 0; i < 10_000_000; i++)
        {
            var id = ClientIdGenerator.Random();

            Assert.That(id, Is.LessThanOrEqualTo(ClientIdGenerator.MaxSafeInteger));
        }
    }
}
