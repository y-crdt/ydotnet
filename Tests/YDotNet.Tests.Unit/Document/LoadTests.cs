using NUnit.Framework;
using YDotNet.Document;

namespace YDotNet.Tests.Unit.Document;

public class LoadTests
{
    [Test]
    public void Load()
    {
        // Arrange and Act
        var doc = new Doc();

        // Assert
        doc.Load();
    }
}
