using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.Events;
using YDotNet.Document.Types.XmlFragments;

namespace YDotNet.Tests.Unit.XmlFragments;

public class ObserveTests
{
    [Test]
    public void ObserveHasTarget()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        XmlFragment? target = null;
        xmlFragment.Observe(e => target = e.Target);

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Assert
        Assert.That(target, Is.Not.Null);
        Assert.That(target.Handle, Is.Not.EqualTo(nint.Zero));
    }

    [Test]
    public void ObserveHasDeltaWhenAddedXmlElementsAndTexts()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        IEnumerable<EventChange>? eventChanges = null;
        xmlFragment.Observe(e => eventChanges = e.Delta.ToArray());

        // Act
        var transaction = doc.WriteTransaction();
        xmlFragment.InsertText(transaction, index: 0);
        xmlFragment.InsertElement(transaction, index: 1, "color");
        xmlFragment.InsertText(transaction, index: 2);
        transaction.Commit();

        // Assert
        transaction = doc.ReadTransaction();

        Assert.That(eventChanges, Is.Not.Null);
        Assert.That(eventChanges.Count(), Is.EqualTo(expected: 1));
        Assert.That(eventChanges.First().Tag, Is.EqualTo(EventChangeTag.Add));
        Assert.That(eventChanges.First().Length, Is.EqualTo(expected: 3));
        Assert.That(eventChanges.First().Values.ElementAt(index: 0).XmlText, Is.Not.Null);
        Assert.That(eventChanges.First().Values.ElementAt(index: 1).XmlElement, Is.Not.Null);
        Assert.That(eventChanges.First().Values.ElementAt(index: 1).XmlElement.Tag(transaction), Is.EqualTo("color"));
        Assert.That(eventChanges.First().Values.ElementAt(index: 2).XmlText, Is.Not.Null);

        transaction.Commit();
    }
}
