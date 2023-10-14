using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Texts;

public class UnobserveTests
{
    [Test]
    public void TriggersWhenTextChangedUntilUnobserved()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("value");
        var called = 0;
        var subscription = text.Observe(_ => called++);

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
