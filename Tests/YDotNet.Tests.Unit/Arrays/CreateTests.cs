using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Arrays;

public class CreateTests
{
    [Test]
    public void Create()
    {
        // Arrange
        var doc = new Doc();

        // Act
        var array = doc.Array("array");
        var transaction = doc.ReadTransaction();
        var length = array.Length(transaction);
        transaction.Commit();

        // Assert
        Assert.That(array.Handle, Is.GreaterThan(nint.Zero));
        Assert.That(length, Is.EqualTo(expected: 0));
    }
}
