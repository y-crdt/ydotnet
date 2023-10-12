using System.Runtime.CompilerServices;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Maps;

namespace YDotNet.Tests.Unit;

public class Program
{
    public static void Main()
    {
        var doc = new Doc();
        var a = doc.Map("A");
        Map b;
        using (var t = doc.WriteTransaction())
        {

            a.Insert(t, "B", Input.Map(new Dictionary<string, Input>()));

            b = a.Get(t, "B").Map;
            b.Insert(t, "C", Input.Double(1));

            a.Remove(t, "B");
        }

        for (var j = 0; j < 10000; j++)
        {
            using (var t = doc.WriteTransaction())
            {
                b.Insert(t, "C", Input.Double(1));
            }
        }

        using (var t = doc.WriteTransaction())
        {
            b.Insert(t, "D", Input.Double(1));
            
            var length = b.Length(t);
        }

            var types = typeof(Program).Assembly.GetTypes();

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        var defaultColor = Console.ForegroundColor;

        var i = 0;
        foreach (var type in types)
        {
            if (!type.IsPublic || type.Namespace?.StartsWith("YDotNet") != true)
            {
                continue;
            }

            var instance = Activator.CreateInstance(type);

            foreach (var method in type.GetMethods(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.DeclaredOnly |
                System.Reflection.BindingFlags.Instance))
            {
                i++;

                if (method.IsStatic || method.GetParameters().Length != 0)
                {
                    continue;
                }

                var attributes = method.GetCustomAttributes(true);

                if (!attributes.Any(x => x.GetType().Name.StartsWith("Test")))
                {
                    continue;
                }

                var name = $"{i:000}: {type.Name}.{method.Name}".PadRight(100, ' ');

                Console.Write(" * {0}", name);

                if (attributes.Any(x => x.GetType().Name.StartsWith("Ignore")))
                {
                    continue;
                }

                GC.Collect();
                try
                {
                    method.Invoke(instance, null);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success");
                    Console.ForegroundColor = defaultColor;
                }
                catch (RuntimeWrappedException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed: {0}", ex.Message);
                    Console.ForegroundColor = defaultColor;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed: {0}", ex.Message);
                    Console.ForegroundColor = defaultColor;
                }
            }
        }
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Console.WriteLine(e.ExceptionObject.ToString());
    }
}
