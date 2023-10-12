namespace YDotNet.Native;

internal record struct NativeWithHandle<T>(T Value, nint Handle) where T : struct;
