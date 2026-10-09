using NUnit.Framework;
using YDotNet.Native.Document;

namespace YDotNet.Tests.Unit.Native;

public class UnmanagedResourceHandleTests
{
    [Test]
    public void ZeroHandleIsInvalid()
    {
        // Arrange
        var handle = new TestHandle(IntPtr.Zero);

        // Act
        var isInvalid = handle.IsInvalid;

        // Assert
        Assert.That(isInvalid, Is.True);
    }

    [Test]
    public void NonZeroHandleIsValid()
    {
        // Arrange
        var handle = new TestHandle(new IntPtr(1));

        // Act
        var isInvalid = handle.IsInvalid;

        // Assert
        Assert.That(isInvalid, Is.False);
    }

    [Test]
    public void ReleasesHandleWhenValid()
    {
        // Arrange
        var handle = new TestHandle(new IntPtr(1));

        // Act
        handle.Dispose();

        // Assert
        Assert.That(handle.DisposeCount, Is.EqualTo(1));
    }

    [Test]
    public void DoesNotReleaseHandleWhenInvalid()
    {
        // Arrange
        var handle = new TestHandle(IntPtr.Zero);

        // Act
        handle.Dispose();

        // Assert
        Assert.That(handle.DisposeCount, Is.EqualTo(0));
    }

    [Test]
    public void PreventsMultipleDisposals()
    {
        // Arrange
        var handle = new TestHandle(new IntPtr(1));

        // Act
        handle.Dispose();
        handle.Dispose();

        // Assert
        Assert.That(handle.DisposeCount, Is.EqualTo(1));
    }

    private sealed class TestHandle : UnmanagedResourceHandle
    {
        public TestHandle(IntPtr handle)
        {
            SetHandle(handle);
        }

        public int DisposeCount { get; private set; }

        protected override bool ReleaseHandle()
        {
            DisposeCount += 1;
            return true;
        }
    }
}
