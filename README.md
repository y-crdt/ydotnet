# YDotNet

YDotNet is a .NET binding for [`y-crdt`](https://github.com/y-crdt/ypy). It provides distributed data types that enable
real-time collaboration between devices. The library is a thin wrapper around Yrs, taking advantage of the safety and performance of Rust.

> 💡 Disclaimer: this project is still early, so it may contain bugs and the API is subject to change. Feel free to
> [open an issue](https://github.com/LSViana/ydotnet/issues/new) if you'd like to report problems or suggest new features.

# Installation

> This section is a WIP, the project has not yet been published to [NuGet](https://www.nuget.org/).

# Getting Started

YDotNet provides the same shared data types as [Yjs](https://docs.yjs.dev/). All objects are shared within a
[`Doc`](https://github.com/LSViana/ydotnet/blob/5c51f761f608d03fc88edaaf31aee4608afe0d3e/YDotNet/Document/Doc.cs) and always get modified within
the scope of a [`Transaction`](https://github.com/LSViana/ydotnet/blob/5c51f761f608d03fc88edaaf31aee4608afe0d3e/YDotNet/Document/Transactions/Transaction.cs).

```csharp
// Set up the local document with some sample data.
var localDoc = new Doc();
var localText = localDoc.Text("name");

var localTransaction = localDoc.WriteTransaction();
localText.Insert(localTransaction, 0, "Y-CRDT");
localTransaction.Commit();

// Set up the remote document.
var remoteDoc = new Doc();
var remoteText = remoteDoc.Text("name");

// Get the remote document state vector.
var remoteTransaction = remoteDoc.WriteTransaction();
var remoteState = remoteTransaction.StateVectorV1();

// Calculate the state diff between the local and the remote document.
localTransaction = localDoc.ReadTransaction();
var stateDiff = localTransaction.StateDiffV1(remoteState);
localTransaction.Commit();

// Apply the state diff to synchronize the remote document with the local changes.
var result = remoteTransaction.ApplyV1(stateDiff);

// Read the text from the remote document.
var text = remoteText.String(remoteTransaction);

// At this point, the `text` variable is "Y-CRDT" and this demonstrates how the two
// documents synchronized their state.
//
// This example does it locally but the same could be done over the Internet, for example.
```

# Development Setup

> This section is a WIP, but you should be able to run the project through the unit tests after
> building a dynamic library for your operating system by adding `cdylib` to the `crate-type` in
> the [`Cargo.toml`](https://github.com/y-crdt/y-crdt/blob/main/yffi/Cargo.toml#L19) file of the
> yffi` library.

# Tests

All tests are located in the [`YDotNet.Tests.Unit`](https://github.com/LSViana/ydotnet/tree/main/Tests/YDotNet.Tests.Unit)
project and should be easily runnable using the command:

```sh
dotnet test
```
