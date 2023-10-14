using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Branches;

public class TextUnobserveDeepTests
{
    [Test]
    public void TriggersWhenChangedUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");
        var called = 0;
        var subscription = text.ObserveDeep(_ => called++);

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "World");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));

        // Act
        subscription.Dispose();

        transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Hello, ");
        transaction.Commit();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
    }
}
