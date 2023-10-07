using YDotNet.Document;
using YDotNet.Document.Cells;

namespace T
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Arrange
            var doc = new Doc();
            var array1 = doc.Array("array-1");

            var transaction = doc.WriteTransaction();
            array1.InsertRange(
                transaction, index: 0, new[]
                {
                Input.Long(value: 2469L),
                Input.Null(),
                Input.Boolean(value: false),
                Input.Map(new Dictionary<string, Input>())
                });

            var map2 = array1.Get(transaction, index: 3).Map;
            var i = Input.Array(Array.Empty<Input>());
            Console.WriteLine(i.InputNative.Tag);
            Console.Out.Flush();
            Thread.Sleep(10000);
            map2.Insert(transaction, "array-3", i);
        }
    }
}
