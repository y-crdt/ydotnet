using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Options;
using YDotNet.Document.Types.Branches;

namespace YDotNet.Tests.Unit.Branches;

public class IdTests
{
    [Test]
    public void ReturnsIdForRootLevelType()
    {
        // Arrange
        var doc = new Doc();
        var branch = doc.Text("text");

        // Act
        var id = branch.Id();

        // Assert
        Assert.That(id, Is.Not.Null);
        Assert.That(id.HasName, Is.True);
        Assert.That(id.HasClientIdAndClock, Is.False);
        Assert.That(id.Name, Is.EqualTo("text"));
        Assert.That(id.ClientId, Is.EqualTo(expected: null));
        Assert.That(id.Clock, Is.EqualTo(expected: null));
    }

    [Test]
    public void ReturnsIdForNestedType()
    {
        // Arrange
        var doc = new Doc(
            new DocOptions
            {
                Id = 37
            });
        var map = doc.Map("map");

        var transaction = map.WriteTransaction();
        map.Insert(transaction, "text", Input.Text(""));
        Branch branch = map.Get(transaction, "text")!.Text;
        transaction.Commit();

        // Act
        var id = branch.Id();

        // Assert
        Assert.That(id, Is.Not.Null);
        Assert.That(id.HasName, Is.False);
        Assert.That(id.HasClientIdAndClock, Is.True);
        Assert.That(id.Name, Is.Null);
        Assert.That(id.ClientId, Is.EqualTo(expected: 37));
        Assert.That(id.Clock, Is.EqualTo(expected: 0));

        // Arrange
        transaction = map.WriteTransaction();
        map.Insert(transaction, "array", Input.Array(Array.Empty<Input>()));
        branch = map.Get(transaction, "array")!.Array;
        transaction.Commit();

        // Act
        id = branch.Id();

        // Assert
        Assert.That(id, Is.Not.Null);
        Assert.That(id.HasName, Is.False);
        Assert.That(id.HasClientIdAndClock, Is.True);
        Assert.That(id.Name, Is.Null);
        Assert.That(id.ClientId, Is.EqualTo(expected: 37));
        Assert.That(id.Clock, Is.EqualTo(expected: 1));
    }
}
