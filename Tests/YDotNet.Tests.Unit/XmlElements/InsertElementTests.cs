using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.XmlElements;

namespace YDotNet.Tests.Unit.XmlElements;

public class InsertElementTests
{
    [Test]
    public void InsertSingleElement()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        var addedXmlElement = xmlElement.InsertElement(transaction, index: 0, "color");
        var childLength = xmlElement.ChildLength(transaction);

        // Assert
        Assert.That(addedXmlElement.Handle, Is.Not.EqualTo(nint.Zero));
        Assert.That(addedXmlElement.Tag(transaction), Is.EqualTo("color"));
        Assert.That(childLength, Is.EqualTo(expected: 1));

        transaction.Commit();
    }

    [Test]
    public void InsertMultipleElements()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        var xmlElement1 = xmlElement.InsertElement(transaction, index: 0, "color");
        var xmlElement2 = xmlElement.InsertElement(transaction, index: 0, "width");
        var xmlElement3 = xmlElement.InsertElement(transaction, index: 0, "height");
        var childLength = xmlElement.ChildLength(transaction);

        // Assert
        Assert.That(xmlElement1.Tag(transaction), Is.EqualTo("color"));
        Assert.That(xmlElement2.Tag(transaction), Is.EqualTo("width"));
        Assert.That(xmlElement3.Tag(transaction), Is.EqualTo("height"));
        Assert.That(childLength, Is.EqualTo(expected: 3));

        transaction.Commit();
    }

    [Test]
    public void InsertAndRemoveMultipleTimes()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");
        XmlElement xmlElement;
        using (var transaction = doc.WriteTransaction())
        {
            xmlElement = xmlFragment.InsertElement(transaction, index: 0, "xml-element");
        }

        // Act
        for (var i = 0; i < 100; i++)
        {
            // Assert
            Assert.DoesNotThrow(() =>
            {
                using (var transaction = doc.WriteTransaction())
                {
                    for (uint j = 0; j < 100; j++)
                    {
                        xmlElement
                            .InsertElement(transaction, index: j, "paragraph")
                            .InsertText(transaction, index: 0)
                            .Insert(transaction, index: 0, "Hello world");
                    }
                }

                using (var transaction = doc.WriteTransaction())
                {
                    xmlElement.RemoveRange(transaction, index: 0, length: 100);
                }
            });
        }
    }
}
