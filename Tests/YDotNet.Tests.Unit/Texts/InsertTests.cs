using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;

namespace YDotNet.Tests.Unit.Texts;

public class InsertTests
{
    [Test]
    public void AddsTextAtBeginning()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("name");

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Hello");
        transaction.Commit();

        // Assert
        transaction = doc.ReadTransaction();

        Assert.That(text.String(transaction), Is.EqualTo("Hello"));
        
        transaction.Commit();
    }

    [Test]
    public void AddsTextAtEnding()
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
        transaction = doc.ReadTransaction();

        Assert.That(text.String(transaction), Is.EqualTo("Hello, world!"));
        
        transaction.Commit();
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
        transaction = doc.ReadTransaction();

        Assert.That(text.String(transaction), Is.EqualTo("Lucas"));
        
        transaction.Commit();
    }

    [Test]
    public void AddsTextAtBeginningWithAttributes()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("name");
        var attributes = new Dictionary<string, Input>
        {
            { "bold", Input.Boolean(value: true) }
        };

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Hello", Input.Object(attributes));
        transaction.Commit();

        // Assert
        transaction = doc.ReadTransaction();

        Assert.That(text.String(transaction), Is.EqualTo("Hello"));
        
        transaction.Commit();
    }

    [Test]
    public void AddsTextAtEndingWithAttributes()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("name");
        var attributes = new Dictionary<string, Input>
        {
            { "bold", Input.Boolean(value: true) }
        };

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Hello");
        text.Insert(transaction, index: 5, ", world!", Input.Object(attributes));
        transaction.Commit();

        // Assert
        transaction = doc.ReadTransaction();

        Assert.That(text.String(transaction), Is.EqualTo("Hello, world!"));
        
        transaction.Commit();
    }

    [Test]
    public void AddsTextAtMiddleWithAttributes()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("name");
        var attributes = new Dictionary<string, Input>
        {
            { "bold", Input.Boolean(value: true) }
        };

        // Act
        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lcas");
        text.Insert(transaction, index: 1, "u", Input.Object(attributes));
        transaction.Commit();

        // Assert
        transaction = doc.ReadTransaction();

        Assert.That(text.String(transaction), Is.EqualTo("Lucas"));
        
        transaction.Commit();
    }
}
