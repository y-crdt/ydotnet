using NUnit.Framework;
using YDotNet.Server.Internal;

namespace YDotNet.Tests.Server.Unit;

public class DelayedWriterTests
{
    [Test]
    public async Task WriteImmediatly()
    {
        // Arrange
        var writeCount = 0;

        var sut = new DelayedWriter(TimeSpan.Zero, TimeSpan.Zero, () =>
        {
            writeCount++;
            return Task.CompletedTask;
        });

        // Act
        sut.Ping();
        await sut.FlushAsync();

        // Assert
        Assert.That(writeCount, Is.EqualTo(1));
    }

    [Test]
    public void WriteAfterDelayNotImmediately()
    {
        // Arrange
        var writeCount = 0;

        var sut = new DelayedWriter(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50), () =>
        {
            writeCount++;
            return Task.CompletedTask;
        });

        // Act
        sut.Ping();

        // Assert
        Assert.That(writeCount, Is.EqualTo(0));
    }

    [Test]
    public async Task WriteAfterDelay()
    {
        // Arrange
        var writeCount = 0;

        var sut = new DelayedWriter(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50), () =>
        {
            writeCount++;
            return Task.CompletedTask;
        });

        // Act
        sut.Ping();
        await Task.Delay(300);

        // Assert
        Assert.That(writeCount, Is.EqualTo(1));
    }

    [Test]
    public async Task WriteAfterDelayOnce()
    {
        // Arrange
        var writeCount = 0;

        var sut = new DelayedWriter(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50), () =>
        {
            writeCount++;
            return Task.CompletedTask;
        });

        // Act
        for (var i = 0; i < 1000; i++)
        {
            sut.Ping();
        }
        await Task.Delay(300);

        // Assert
        Assert.That(writeCount, Is.EqualTo(1));
    }

    [Test]
    public async Task WriteAfterDelayAgain()
    {
        // Arrange
        var writeCount = 0;

        var sut = new DelayedWriter(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50), () =>
        {
            writeCount++;
            return Task.CompletedTask;
        });

        // Act
        sut.Ping();
        await Task.Delay(300);

        sut.Ping();
        await Task.Delay(300);

        // Assert
        Assert.That(writeCount, Is.EqualTo(2));
    }
}
