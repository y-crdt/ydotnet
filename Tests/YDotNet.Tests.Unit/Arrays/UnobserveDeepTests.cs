using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.Arrays;

public class UnobserveDeepTests
{
    [Test]
    public void TriggersWhenArrayChangedUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var array = doc.Array("array");
        var called = 0;
        var subscription = array.ObserveDeep(_ => called++);

        // Act
        var transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.Long(value: 2469L) });
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));

        // Act
        array.UnobserveDeep(subscription);

        transaction = doc.WriteTransaction();
        array.InsertRange(transaction, index: 0, new[] { Input.Long(value: -420L) });
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
    }
}
