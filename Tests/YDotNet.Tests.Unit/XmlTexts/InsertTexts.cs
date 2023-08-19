using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.XmlTexts;

public class InsertTexts
{
    [Test]
    public void InsertWithoutAttributes()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        transaction.Commit();

        // Assert
        Assert.That(xmlText.Length(transaction), Is.EqualTo(expected: 5));

        // TODO [LSViana] Replace this assertion with XmlText.String() for more reliable verification.

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 5, " 💻");
        transaction.Commit();

        // Assert
        Assert.That(xmlText.Length(transaction), Is.EqualTo(expected: 8));

        // TODO [LSViana] Replace this assertion with XmlText.String() for more reliable verification.
    }

    [Test]
    public void InsertWithAttributes()
    {
        // Arrange
        var doc = new Doc();
        var xmlText = doc.XmlText("xml-text");

        // Act
        var transaction = doc.WriteTransaction();
        xmlText.Insert(
            transaction,
            index: 0,
            "Lucas",
            Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        transaction.Commit();

        // Assert
        Assert.That(xmlText.Length(transaction), Is.EqualTo(expected: 5));

        // TODO [LSViana] Replace this assertion with XmlText.String() for more reliable verification.

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(
            transaction,
            index: 5,
            " 💻",
            Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        transaction.Commit();

        // Assert
        Assert.That(xmlText.Length(transaction), Is.EqualTo(expected: 8));

        // TODO [LSViana] Replace this assertion with XmlText.String() for more reliable verification.
    }
}
