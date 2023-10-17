# Decision

## 2023-10-16 Publicly exposed elements must be documented

The documentation for `class` and `struct` elements is important to instruct external users on how to use the library.
These elements are normally under the `YDotNet.Document` namespace.

The `internal` or `private` classes, however, must not be documented. It's allowed to add comments explaining why such
code works the way it does, though. Bigger decisions must be documented in this document.

## 2023-10-16 No circular dependencies between native and types

There is the natural dependency between document types to native, because they read data, but there is also a dependency
in the other direction because some native types can actually generate stuff with `ToXXX` methods.
This is a circular dependency between namespaces that should in general be avoided.

Furthermore some types do not rely on the native types and calculate pointers manually,
e.g. [`EventDeltaAttribute`](https://github.com/LSViana/ydotnet/blob/main/YDotNet/Document/Types/Events/EventDeltaAttribute.cs).

Therefore, you must ensure that:

1. There is always a native type.

2. Native types are passed to the CLR types via constructors.

3. Native types are responsible to follow pointers,
e.g. [`UndoEventNative`](https://github.com/SebastianStehle/ydotnet/blob/main/YDotNet/Native/UndoManager/Events/UndoEventNative.cs#L18C5-L27C1)
and everything around reading memory is part of the native part. These reading operations are provided via methods to
indicate that it is not a property and not part of the struct itself.

Some objects also need the handle and the native `struct`. Therefore a new type has been introduced:

```csharp
internal record struct NativeWithHandle<T>(T Value, nint Handle) where T : struct;
```

[View the source code](https://github.com/SebastianStehle/ydotnet/blob/main/YDotNet/Native/NativeWithHandle.cs).

## 2023-10-16 Objects must check for disposal before every operation

When we access a pointer that is not pointing to a valid `struct` anymore, the process can crash.
This is very hard to catch and debug (because logging does not work) and therefore needs to be avoided at all costs.

If an object has been disposed, it must throw an `ObjectDisposedException`.
This task is still work in progress and a lot of changes have been done to make this work.

1. The `Doc` has a [`TypeCache`](https://github.com/SebastianStehle/ydotnet/blob/main/YDotNet/Infrastructure/TypeCache.cs),
which is basically a map from handle to type. This type cache ensures that there is only one object for each pointer.
The `TypeCache` uses a `WeakMap` to ensure that unused objects can be garbage collected.

2. The `TypeCache` gives the option to mark object as disposed, for example,
when an Yrs event indicates an object has been removed.
   - This is still work in progress because Yrs does not provide all necessary information yet.

3. Because of the `TypeCache` each object needs a reference to the document
to be able to construct types using the document.

4. Some objects are also deleted from beginning. If you get an old value from an event that points to a removed `Map`,
this `Map` is not usable anymore and should be marked as disposed. This is similar for removed sub-documents.

##  2023-10-16 Objects must be disposed only once

`Transaction`s and every type that can be disposed must not be disposed twice.
This can cause an application crash when:

1. A dangling pointer is used after the first dispose.
2. A second attempt to dispose is executed (which usually causes the exit code 139).

New base classes have been introduced to ensure that this does not happen.

##  2023-10-16 Not all transaction cases are catched by yrs

The application crashes if a new root object is created while a `Transaction` is open.

Therefore, the `Doc` and `Transaction` classes have a mechanism to track the amount of open `Transaction`s.
Tests have been added for this case.

##  2023-10-16 Memory must be managed by `DisposableHandle`

Before the addition of `DisposableHandle`, the classes that needed to allocate memory to perform native calls would
do something like this:

```csharp
var handle = MemoryWriter.WriteUtf8String(value);

// Perform native call

MemoryWriter.Release(value);
```

To improve that, the `MemoryWriter` class now returns instances of `DisposableHandle`:

```csharp
internal sealed record DisposableHandle(nint Handle) : IDisposable
{
    public void Dispose()
    {
        if (Handle != nint.Zero)
        {
            Marshal.FreeHGlobal(Handle);
        }
    }
}
```

This means the memory is now released automatically by `DisposableHandle` when it's disposed. For example:

```csharp
public void Insert(Transaction transaction, string key, Input input)
{
    ThrowIfDisposed();

    using var unsafeKey = MemoryWriter.WriteUtf8String(key);
    using var unsafeValue = MemoryWriter.WriteStruct(input.InputNative);

    MapChannel.Insert(Handle, transaction.Handle, unsafeKey.Handle, unsafeValue.Handle);
}
```

##  2023-10-16 `MapEntry` was replaced by `KeyValuePair`

This type is used by Yrs to transport key-value pairs. In .NET, there is the `KeyValuePair` struct for that purpose.
Therefore, we got rid of that to also build the `Map` structures more similar to the .NET built-in ones.

Whenever we deal with key-value pairs, we also expose them as dictionaries.
For example, in the events to make the access by key more ergonomic.

## 2023-10-16 Replace `IEnumerable` with `ReadOnlyCollection`

Some types, like `EventChanges`, represent read-only collections.
This has been optimized by inheriting from `ReadOnlyCollection` that are designed for that. Same goes for dictionaries.

This provides more methods and properties (like `Count`) for the user and also removes the amount of code.