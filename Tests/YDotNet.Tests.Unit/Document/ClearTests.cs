using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class ClearTests
{
    [Test]
    public void Clear()
    {
        // Arrange and Act
        var doc = new Doc();

        // Assert
        doc.Clear();
    }
}
