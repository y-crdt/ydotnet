<p align="center">
  <img width="128" height="128" src="https://github.com/LSViana/ydotnet/assets/21217790/51be1008-7bac-4bb0-b5bb-d4c9f5ab6d7d" />
</p>

# YDotNet

YDotNet is a .NET binding for [`y-crdt`](https://github.com/y-crdt/ypy). It provides distributed data types that enable
real-time collaboration between devices. The library is a thin wrapper around Yrs, taking advantage of the safety and performance of Rust.

> 💡 Disclaimer: this project is still early, so it may contain bugs and the API is subject to change. Feel free to
> [open an issue](https://github.com/LSViana/ydotnet/issues/new) if you'd like to report problems or suggest new features.

# Demo

Check the following video:

https://github.com/LSViana/ydotnet/assets/21217790/cdb6023d-25d1-4951-82ae-b079ddbd8d26

# Installation

For every scenario, you must start by installing the core of the library.

To do so, in the project directory (where you `.csproj` lives), execute:

```shell
dotnet add package YDotNet
```

Then, install the platform-specific package in order to get the binaries.

| Package                                                                     | Platform |
|-----------------------------------------------------------------------------|----------|
| [YDotNet.Native.Win32](https://www.nuget.org/packages/YDotNet.Native.Win32) | Windows  |
| [YDotNet.Native.Linux](https://www.nuget.org/packages/YDotNet.Native.Linux) | Linux    |
| [YDotNet.Native.MacOS](https://www.nuget.org/packages/YDotNet.Native.MacOS) | macOS    |

And you may also install the following packages to get extra features.

| Package                                                                               | Description                                                  |
|---------------------------------------------------------------------------------------|--------------------------------------------------------------|
| [YDotNet.Extensions](https://www.nuget.org/packages/YDotNet.Extensions)               | Extension methods to make some operations easier.            |
| [YDotNet.Server](https://www.nuget.org/packages/YDotNet.Server)                       | (to be added)                                                |
| [YDotNet.Server.WebSockets](https://www.nuget.org/packages/YDotNet.Server.WebSockets) | Use WebSockets as the communication channel between clients. |
| [YDotNet.Server.MongoDB](https://www.nuget.org/packages/YDotNet.Server.MongoDB)       | Use MongoDB as a persistence layer.                          |
| [YDotNet.Server.Redis](https://www.nuget.org/packages/YDotNet.Server.Redis)           | Use Redis as a persistence layer.                            |

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

To contribute with this library, you'll need to install the following tools:

- [.NET SDK](https://dotnet.microsoft.com/download/dotnet/)
- [Rust](https://www.rust-lang.org/tools/install)

Then you should clone the [`y-crdt`](https://github.com/y-crdt/y-crdt) repository. With the repository
cloned and the tools installed, you'll be able to:

1. Make changes to the Rust or C# library;
2. Re-build the Rust and C# binaries;
    - Be aware that you'll need to use
      [`crate-type=cdylib`](https://github.com/y-crdt/y-crdt/blob/main/yffi/Cargo.toml#L19)
      on the `Cargo.toml` file to get a dynamic library that's callable by C#.
3. Test your changes and repeat.

Then you're ready to go! Feel free to contribute, open issues, and ask questions.

# Tests

All tests are located in the [`YDotNet.Tests.Unit`](https://github.com/LSViana/ydotnet/tree/main/Tests/YDotNet.Tests.Unit)
project and should be easily runnable using the command:

```sh
dotnet test
```
