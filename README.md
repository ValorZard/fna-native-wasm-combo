# FNA Native Wasm Combo
This project shows off a way to have FNA compile to both Native and Wasm.

However, the big catch is that we need to clone FNA twice, and then have one version of FNA be patched in order to compile to WASM.

In order to do the setup for both wasm and native, run ``dotnet run setup.cs``

## running the native version:
- First, grab the FNA binaries from [fnalibs-dailies](https://github.com/FNA-XNA/fnalibs-dailies), and put the ones that fit the architecture you are on right next to where your executable will be.
- (For example, if you are on x64, copy the binaries in the x64 folder and put them in ``.\FNANativeRunner\bin\Debug\net10.0``)
- Then, do ``dotnet run --project FNANativeRunner``

## running the web version
- do ``dotnet run do_wasm.cs serve``
- (if you need to clean, you can do ``dotnet run do_wasm.cs serve clean``)
- In case this is the first time running (or if you want a ``clean`` build), it will fetch the patched FNA binaries from [FNA-WASM-Build](https://github.com/r58Playz/FNA-WASM-Build) and put them in ``FNAWasmRunner/statics``
- The ones it will grab are:
    - ``FNA3D.a``
    - ``FAudio.a``
    - ``SDL3.a`` 
    - and ``libmojoshader.a``
- Note: sometimes the script might fail with an error like this
```
CSC : error CS2012: Cannot open 'C:\workspace\fna-game\FNAWasm\obj_core\Release\net8.0\FNA.dll' for writing -- The process cannot access the file 'C:\workspace\fna-game\FNAWasm\obj_core\Release\net8.0\FNA.dll' because it is being used by another process.; file may be locked by 'VBCSCompiler' (54356)
```
- Just retry the script again and it should work
- The web version will exist on ``http://localhost:5000/``

## Notes
Thanks to @r58Playz for the inspiration/borrowed code from (FNA-Wasm-Threads)[https://github.com/r58Playz/fna-wasm-threads] to make this work