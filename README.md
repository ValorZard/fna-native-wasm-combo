# FNA Native Wasm Combo
This project shows off a way to have FNA compile to both Native and Wasm.

However, the big catch is that we need to clone FNA twice, and then have one version of FNA be patched in order to compile to WASM.

In order to do the setup for both wasm and native, run ``dotnet run setup.cs``

For the web version, it will do the following setup
- fetch the patched FNA binaries from [FNA-WASM-Build](https://github.com/r58Playz/FNA-WASM-Build) and put them in ``FNAWasmRunner/statics``
- The ones it will grab are:
    - ``FNA3D.a``
    - ``FAudio.a``
    - ``SDL3.a`` 
    - and ``libmojoshader.a``

Then, to run the native version just do 
- ``dotnet run --project FNANativeRunner``

To run the web version
- do ``dotnet run do_wasm.cs serve``
- (if you need to clean, you can do ``dotnet run do_wasm.cs serve clean``)
- Note: sometimes the script might fail with an error like this
```
CSC : error CS2012: Cannot open 'C:\workspace\fna-game\FNAWasm\obj_core\Release\net8.0\FNA.dll' for writing -- The process cannot access the file 'C:\workspace\fna-game\FNAWasm\obj_core\Release\net8.0\FNA.dll' because it is being used by another process.; file may be locked by 'VBCSCompiler' (54356)
```
- Just retry the script again and it should work
- The web version will exist on ``http://localhost:5000/``