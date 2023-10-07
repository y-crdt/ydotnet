using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Events;

namespace T
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Arrange
            var doc = new Doc();

            AfterTransactionEvent? afterTransactionEvent = null;
            var called = 0;

            var text = doc.Text("country");
            var subscription = doc.ObserveAfterTransaction(
                e =>
                {
                    called++;
                    afterTransactionEvent = e;
                });

            // Act
            var transaction = doc.WriteTransaction();
            text.Insert(transaction, index: 0, "Brazil");
            transaction.Commit();
            // Act
            afterTransactionEvent = null;
            transaction = doc.WriteTransaction();
            text.Insert(transaction, index: 0, "Great ");
            transaction.Commit();

            // Act
            afterTransactionEvent = null;
            doc.UnobserveAfterTransaction(subscription);

            transaction = doc.WriteTransaction();
            text.Insert(transaction, index: 0, "The ");
            transaction.Commit();
        }
    }
}
