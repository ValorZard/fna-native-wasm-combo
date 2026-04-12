clean:
	rm -rvf obj bin FNA || true

build:
	dotnet publish -c Release -v d
	# fix mono init with -sWASMFS enabled
	sed -i 's/FS_createPath("\/","usr\/share",!0,!0)/FS_createPath("\/usr","share",!0,!0)/' bin/Release/net10.0/publish/wwwroot/_framework/dotnet.runtime.*.js
	# automatically force transfer of canvas matching selector `.canvas` (class canvas) to deputy thread (c# managed main thread)
	sed -i 's/var offscreenCanvases={};/var offscreenCanvases={};if(globalThis.window\&\&!window.TRANSFERRED_CANVAS){transferredCanvasNames=[".canvas"];window.TRANSFERRED_CANVAS=true;}/' bin/Release/net10.0/publish/wwwroot/_framework/dotnet.native.*.js

serve: build
	python3 tools/serve.py
