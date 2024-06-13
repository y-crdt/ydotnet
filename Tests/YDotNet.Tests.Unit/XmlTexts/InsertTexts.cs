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
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 0, "Lucas");
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lucas"));

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(transaction, index: 5, " 💻");
        text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("Lucas 💻"));
    }

    [Test]
    public void InsertWithAttributes()
    {
        // Arrange
        var doc = new Doc();
        var xmlFragment = doc.XmlFragment("xml-fragment");

        var transaction = doc.WriteTransaction();
        var xmlText = xmlFragment.InsertText(transaction, index: 0);
        transaction.Commit();

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(
            transaction,
            index: 0,
            "Lucas",
            Input.Object(
                new Dictionary<string, Input>
                {
                    { "bold", Input.Boolean(value: true) }
                }));
        var text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<bold>Lucas</bold>"));

        // Act
        transaction = doc.WriteTransaction();
        xmlText.Insert(
            transaction,
            index: 5,
            " 💻",
            Input.Object(
                new Dictionary<string, Input>
                {
                    { "italic", Input.Boolean(value: true) }
                }));
        text = xmlText.String(transaction);
        transaction.Commit();

        // Assert
        Assert.That(text, Is.EqualTo("<bold>Lucas</bold><italic> 💻</italic>"));
    }
}
