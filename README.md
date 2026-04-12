# FNA Native Wasm Combo
This project shows off a way to have FNA compile to both Native and Wasm.

However, the big catch is that we need to clone FNA twice, and then have one version of FNA be patched in order to compile to WASM.

In order to do the setup for both wasm and native, run ``dotnet run setup.cs``

Then, to run the native version just do 
- ``dotnet run --project FNANativeRunner``

To run the web version
- Make sure to put the patched FNA binaries from [FNA-WASM-Build](https://github.com/r58Playz/FNA-WASM-Build) in FNAWasmRunner/statics
- then, do ``cd FNAWasmRunner`` and then ``make serve``
- The web version will exist on ``http://localhost:5000/``