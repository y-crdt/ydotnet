using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Events;

namespace YDotNet.Tests.Unit.Document;

public class ClearTests
{
    [Test]
    public void Clear()
    {
        // Arrange and Act
        var doc = new Doc();

        // Assert
        doc.Clear();
    }

    [Test]
    public void TriggersWhenObserved()
    {
        // Arrange
        var doc = new Doc();

        ClearEvent? clearEvent = null;
        var called = 0;

        var subscription = doc.ObserveClear(
            e =>
            {
                called++;
                clearEvent = e;
            });

        // Act
        doc.Clear();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 1));
        Assert.That(clearEvent, Is.Not.Null);
        Assert.That(clearEvent.Doc.Id, Is.EqualTo(doc.Id));
    }

    [Test]
    public void DoesNotTriggerWhenUnobserved()
    {
        // Arrange
        var doc = new Doc();

        ClearEvent? clearEvent = null;
        var called = 0;

        var subscription = doc.ObserveClear(
            e =>
            {
                called++;
                clearEvent = e;
            });

        // Act
        subscription.Dispose();
        doc.Clear();

        // Assert
        Assert.That(called, Is.EqualTo(expected: 0));
        Assert.That(clearEvent, Is.Null);
    }
}
