using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Types.Texts;

namespace YDotNet.Tests.Unit.Branches.BranchId;

public class GetTests
{
    [Test]
    public void ReturnsTheCorrectBranchWhenAlive()
    {
        // Arrange
        var doc = new Doc();
        var branch = doc.Text("text");
        var branchId = branch.Id();

        // Act
        var transaction = doc.ReadTransaction();
        var newBranch = branchId.Get(transaction);
        transaction.Commit();

        // Assert
        Assert.That(newBranch, Is.Not.Null);
        Assert.That(newBranch, Is.TypeOf<Text>());
        Assert.That(newBranch, Is.EqualTo(branch));
    }

    [Test]
    [Ignore("TODO: Check how to mark a Branch as deleted.")]
    public void ReturnsTheCorrectBranchWhenNotAlive()
    {
    }
}
