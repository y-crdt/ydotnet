# Decision

## 2023-10-16 Do not document internal classes via XML ocmments

They are tedious and annoying to write and have to be updated regularly. Thefore it does not make sense to add any XML comments for XML types. We should document code and the reason why something has been implemented in a certain way.

## 2023-10-16 No circular dependencies between native and types

There is the natural dependency between document types to native, because they read data, but there is also a dependency in the other direction because some native types can actually generate stuff with `ToXXX` methods. This is a circular dependency between namespaces that should in general be avoided.

Furthermore some types do not rely on the native types and calculate pointers manually, e.g. https://github.com/LSViana/ydotnet/blob/main/YDotNet/Document/Types/Events/EventDeltaAttribute.cs

Therefore I ensured that...

1. There is always a native type.
2. Native types are passed to the CLR types via constructors.
3. Native types are responsible to follow pointers, e.g. https://github.com/SebastianStehle/ydotnet/blob/main/YDotNet/Native/UndoManager/Events/UndoEventNative.cs#L18C5-L27C1 and everything around reading memory is part of the native part. These reading operations are provided via methods to indicate that it is not a property and not part of the struct itself.

Some objects also need the handle and the native struct. Therefore a new type has been introduced:

```csharp
internal record struct NativeWithHandle<T>(T Value, nint Handle) where T : struct;
```

https://github.com/SebastianStehle/ydotnet/blob/main/YDotNet/Native/NativeWithHandle.cs

## 2023-10-16 The library should not provide access to invalid objects

When we access a pointer that is not pointing to a valid struct anymore, the process can crash. This is very hard to catch and debug (because logging does not work) and therefore needs to be avoided at all costs.

If an object has been deleted it shoudl throw an `ObjectDisposedException`. This task is still work in progress but a lot of changes have been done to make this work.

1. The document has type cache, which is basically a map from handle to type: https://github.com/SebastianStehle/ydotnet/blob/main/YDotNet/Infrastructure/TypeCache.cs. This type cache ensures that there is only one object for each pointer. The type cache uses a weak map to ensure that unused objects can be garbage collected.
2. The type cache gives the option to mark object as deleted, for example when we receive an event from yrs that an object has been removed. This is still work in progress, because yrs does not provide all relevant information yet.
3. Because of the type cache each object needs a reference to the document to be able to construct types using the document.
4. Furthermore some objects are also deleted from beginning. If you get an old value from an event that points to a removed map, this map is not usable anymore and should be marked as deleted. Similar for removed subdocuments.

##  2023-10-16 Objects must not be disposed twice

Transactiosn and every type that can be disposed must not be disposed twice. This can cause an application crash when a danging pointer is used after the first dispose. New base classes have been introduced to ensure that this does not happen.

##  2023-10-16 Not all transaction cases are catched by yrs

When you create a new root object while a transaction is still pending the application crashs. Therefore we also implement a mechanism in .NET to track the amount of pending transactions. Tests have been added for this case.

##  2023-10-16 The types should not be responsible to release memory properly

When you call a native method in a type it is necessary to allocate unmanaged memory to pass a pointer to string or buffer. Before the change the methods are responsible to release the memory and take care about that in every single place.

To improve that the memory writer returns a new type now:

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

When using the dispose pattern properly the type methods do not have to handle this anymore. For example:

```csharp
public void Insert(Transaction transaction, string key, Input input)
{
    ThrowIfDisposed();

    using var unsafeKey = MemoryWriter.WriteUtf8String(key);
    using var unsafeValue = MemoryWriter.WriteStruct(input.InputNative);

    MapChannel.Insert(Handle, transaction.Handle, unsafeKey.Handle, unsafeValue.Handle);
}
```

This change also made a simplification of the input class possible and eliminates the inherited type of inputs.

##  2023-10-16 MapEntry is useless

This type is from yrs to transport key and value. In .NET we are used to leverage the `KeyValuePair` struct from that. Therefore we got rid of that, to also build the Map structures more similar to .NET.

Whenevery we deal with key-value pairs we also expose them as dictionaries, for example in events to make the access by key as fast as possible.

## 2023-10-16 Do not implement IEnumerable when not needed

Some types like `EventChanges` represent readonly collections. They are implemented manually. This has been optimized by inheriting from readonly collections that are designed for that. Same for dictionaries.

This provides more methods and properties (like Count) for the user and also removes the amount of code.