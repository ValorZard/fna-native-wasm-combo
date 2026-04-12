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
- do ``cd FNAWasmRunner`` and then ``make serve``
- The web version will exist on ``http://localhost:5000/``