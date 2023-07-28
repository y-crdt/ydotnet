using NUnit.Framework;

namespace YDotNet.Tests.Unit.Document;

public class DestroyTests
{
    [Test]
    [Ignore("Approach to ensure document destruction is not clear yet.")]
    public void Dispose()
    {
        // TODO [LSViana] Check how to test this method.
        //
        // Before, it was setting `Doc.Handle = nint.zero` but this doesn't necessarily make
        // sense because this isn't a feature of the native library, then it was removed.
    }
}
