using System.Collections.Generic;
using NUnit.Framework;
using YDotNet.Infrastructure;

namespace YDotNet.Tests.Unit.Infrastructure;

public class ClientIdGeneratorTests
{
    [Test]
    public void HasCorrectMaxValue()
    {
        // 2^53 - 1 (JavaScript Number.MAX_SAFE_INTEGER). NOTE: `2 ^ 53` is XOR in C#, not exponentiation.
        Assert.That(ClientIdGenerator.MaxSafeInteger, Is.EqualTo(9007199254740991UL));
    }

    [Test]
    public void RandomStaysWithinSafeIntegerRange()
    {
        for (var i = 0; i < 1000; i++)
        {
            Assert.That(ClientIdGenerator.Random(), Is.LessThanOrEqualTo(ClientIdGenerator.MaxSafeInteger));
        }
    }

    [Test]
    public void RandomProducesManyDistinctValues()
    {
        var ids = new HashSet<ulong>();

        for (var i = 0; i < 1000; i++)
        {
            ids.Add(ClientIdGenerator.Random());
        }

        // The previous implementation masked with the XOR'd constant (= 54 = 0b110110), collapsing the
        // output to only 16 distinct values. A correct generator yields near-unique ids.
        Assert.That(ids, Has.Count.GreaterThan(990));
    }
}
