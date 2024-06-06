using System.Runtime.InteropServices;

namespace YDotNet.Native;

[StructLayout(LayoutKind.Sequential)]
internal record struct NativeWithHandle<T>(T Value, nint Handle)
    where T : struct;
