using System.Runtime.CompilerServices;

namespace YDotNet.Tests.Unit;

public class Program
{
    public static void Main()
    {
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
                    for (var j = 0; j < 1000; j++)
                    {
                        method.Invoke(instance, null);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success");
                    Console.ForegroundColor = defaultColor;
                }
                catch (RuntimeWrappedException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed: {0}", ex);
                    Console.ForegroundColor = defaultColor;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed: {0}", ex);
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
