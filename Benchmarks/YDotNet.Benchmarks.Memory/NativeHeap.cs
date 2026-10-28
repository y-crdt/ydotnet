using System.Runtime.InteropServices;

namespace YDotNet.Benchmarks.Memory;

/// <summary>
///     Reads the number of bytes currently allocated through the C allocator (<c>malloc</c>).
/// </summary>
/// <remarks>
///     Yrs and <see cref="Marshal.AllocHGlobal(int)" /> both allocate through <c>malloc</c> on macOS and Linux, so the
///     allocator statistics count live native bytes exactly, unlike the resident set size of the process.
/// </remarks>
public static class NativeHeap
{
    private static readonly Func<long>? Reader = CreateReader();

    /// <summary>
    ///     Gets a value indicating whether the native heap can be measured on the current platform.
    /// </summary>
    public static bool IsSupported => Reader != null;

    /// <summary>
    ///     Gets the number of bytes currently allocated in the native heap.
    /// </summary>
    /// <returns>The number of bytes in use.</returns>
    public static long BytesInUse()
    {
        if (Reader == null)
        {
            throw new PlatformNotSupportedException("The native heap cannot be measured on this platform.");
        }

        return Reader();
    }

    private static Func<long>? CreateReader()
    {
        try
        {
            if (OperatingSystem.IsMacOS())
            {
                MacOS.ReadBytesInUse();

                return MacOS.ReadBytesInUse;
            }

            if (OperatingSystem.IsLinux())
            {
                Linux.ReadBytesInUse();

                return Linux.ReadBytesInUse;
            }
        }
        catch (Exception ex) when (ex is EntryPointNotFoundException or DllNotFoundException)
        {
            // For example, musl does not export mallinfo2.
        }

        return null;
    }

    private static class MacOS
    {
        public static long ReadBytesInUse()
        {
            // A null zone sums the statistics of all malloc zones.
            malloc_zone_statistics(nint.Zero, out var statistics);

            return (long)statistics.SizeInUse;
        }

        [DllImport("libc")]
        private static extern void malloc_zone_statistics(nint zone, out MallocStatistics statistics);

        [StructLayout(LayoutKind.Sequential)]
        private struct MallocStatistics
        {
            public uint BlocksInUse;
            public nuint SizeInUse;
            public nuint MaxSizeInUse;
            public nuint SizeAllocated;
        }
    }

    private static class Linux
    {
        public static long ReadBytesInUse()
        {
            var info = mallinfo2();

            // Allocated chunks plus chunks that were large enough to be served by mmap.
            return (long)(info.Uordblks + info.Hblkhd);
        }

        [DllImport("libc")]
        private static extern MallInfo2 mallinfo2();

        [StructLayout(LayoutKind.Sequential)]
        private struct MallInfo2
        {
            public nuint Arena;
            public nuint Ordblks;
            public nuint Smblks;
            public nuint Hblks;
            public nuint Hblkhd;
            public nuint Usmblks;
            public nuint Fsmblks;
            public nuint Uordblks;
            public nuint Fordblks;
            public nuint Keepcost;
        }
    }
}
