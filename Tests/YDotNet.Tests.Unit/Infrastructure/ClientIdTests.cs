using NUnit.Framework;
using YDotNet.Infrastructure;

namespace YDotNet.Tests.Unit.Infrastructure;

public class ClientIdTests
{
    [Test]
    public void TestSatefy()
    {
        for (var i = 0; i < 10_000_000; i++)
        {
            var id = ClientId.GetRandom();

            Assert.That(id, Is.LessThanOrEqualTo(ClientId.MaxSafeInteger));
        }
    }
}
