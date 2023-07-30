using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Texts;

public class InsertTests
{
    [Test]
    public void AddsTextAtBeginningText()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("name");

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Hello");
        transaction.Commit();

        // Assert
        Assert.That(text.String(transaction), Is.EqualTo("Hello"));
    }

    [Test]
    public void AddsTextAtEndingText()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("name");

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Hello");
        text.Insert(transaction, index: 5, ", world!");
        transaction.Commit();

        // Assert
        Assert.That(text.String(transaction), Is.EqualTo("Hello, world!"));
    }

    [Test]
    public void AddsTextAtMiddle()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("name");

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lcas");
        text.Insert(transaction, index: 1, "u");
        transaction.Commit();

        // Assert
        Assert.That(text.String(transaction), Is.EqualTo("Lucas"));
    }
}
