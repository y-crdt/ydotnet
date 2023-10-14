using NUnit.Framework;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;

namespace YDotNet.Tests.Unit.Branches;

public class TextObserveDeepTests
{
    [Test]
    [Ignore("The feature under test does not exist on Yrs yet.")]
    public void ObserveDeepHasPathWhenAdded()
    {
        // Arrange
        var doc = new Doc();
        var text = doc.Text("text");

        var transaction = doc.WriteTransaction();
        text.Insert(transaction, index: 0, "Lucas");
        text.InsertEmbed(
            transaction, index: 5, Input.Collection(
                new[]
                {
                    Input.Long(value: 2469L),
                    Input.Array(
                        new[]
                        {
                            Input.Long(value: 420L)
                        })
                }));
        text.InsertEmbed(transaction, index: 6, Input.String("Viana"));
        transaction.Commit();

        IEnumerable<EventPathSegment>? pathSegments = null;

        var subscription = text.ObserveDeep(events => pathSegments = events.First().TextEvent.Path.ToArray());

        // Act
        transaction = doc.WriteTransaction();
        text.RemoveRange(transaction, index: 5, length: 1);
        text.InsertEmbed(
            transaction, index: 5, Input.Collection(
                new[]
                {
                    Input.Long(value: 2469L),
                    Input.Array(
                        new[]
                        {
                            Input.Long(value: 1337L)
                        })
                }));
        transaction.Commit();

        // Assert
        Assert.That(pathSegments.Count(), Is.EqualTo(expected: 1));
    }
}
