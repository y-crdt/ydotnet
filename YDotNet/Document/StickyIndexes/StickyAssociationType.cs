namespace YDotNet.Document.StickyIndexes;

/// <summary>
///     Association type used by a <see cref="StickyIndex" />.
/// </summary>
/// <remarks>
///     <para>
///         In general, a <see cref="StickyIndex" /> refers to a cursor space between two elements (eg. "ab.c" where "abc"
///         is our string and `.` is the <see cref="StickyIndex" /> placement).
///     </para>
///     <para>
///         In a situation when another client is updating a collection concurrently, a new set of elements may be inserted
///         into that space, expanding it in the result. In such case, the <see cref="StickyAssociationType" /> tells us if
///         the <see cref="StickyIndex" /> should stick to location before or after referenced index.
///     </para>
/// </remarks>
public enum StickyAssociationType : sbyte
{
    /// <summary>
    ///     The corresponding <see cref="StickyIndex" /> points to space after the referenced element.
    /// </summary>
    After = 0,

    /// <summary>
    ///     The corresponding <see cref="StickyIndex" /> points to space before the referenced element.
    /// </summary>
    Before = -1,
}
