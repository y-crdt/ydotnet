using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Branches;

public class AliveTests
{
    [Test]
    public void ReturnsTrueWhenAlive()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");

        // Act
        var alive = text.Alive();

        // Assert
        Assert.That(alive, Is.True);
    }

    [Test]
    [Ignore("TODO: Check how to mark a Branch as deleted.")]
    public void ReturnsFalseWhenNotAlive()
    {
    }
}
