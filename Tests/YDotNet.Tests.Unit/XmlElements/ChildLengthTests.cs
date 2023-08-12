using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.XmlElements;

public class ChildLengthTests
{
    [Test]
    public void InitialLengthIsZero()
    {
        // Arrange
        var doc = new Doc();
        var xmlElement = doc.XmlElement("xml-element");

        // Act
        var transaction = doc.ReadTransaction();
        var childLength = xmlElement.ChildLength(transaction);
        transaction.Commit();

        // Assert
        Assert.That(childLength, Is.EqualTo(expected: 0));
    }
}
