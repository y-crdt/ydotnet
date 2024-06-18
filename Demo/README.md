If you are launching this demo from scratch, follow the steps:

+ Install .Net 7.0
+ Install Rust
+ Install Node.JS and npm
+ Install vite and npx
+ Build UI via "npx vite build" - launch from Client folder
+ Clone repo (https://github.com/y-crdt/y-crdt)
+ Build Rust library "libyrdt.dylib"
+ Copy "libyrdt.dylib" to proper folder - check Demo.csproj for reference, for me it was Demo/bin/Debug/net7.0/runtimes/osx/native
+ Build and run Demo.csproj - it will be your server, with in-memory persistence
+ go to Client folder and start your UI via "npm run dev"



