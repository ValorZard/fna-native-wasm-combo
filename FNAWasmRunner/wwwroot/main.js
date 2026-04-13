
const wasm = await eval(`import("/_framework/dotnet.js")`);
const dotnet = wasm.dotnet;

const preloadContentAssets = async (module) => {
	const assets = ["popsicle.png"];

	if (typeof module.FS_createPath !== "function" || typeof module.FS_createDataFile !== "function") {
		console.warn("WASM FS helpers are unavailable; content preload skipped.");
		return;
	}

	module.FS_createPath("/", "Content", true, true);

	for (const asset of assets) {
		const assetPath = `/Content/${asset}`;
		const response = await fetch(assetPath);
		if (!response.ok) {
			throw new Error(`Failed to fetch ${assetPath}: ${response.status} ${response.statusText}`);
		}

		const bytes = new Uint8Array(await response.arrayBuffer());
		if (typeof module.FS_unlink === "function") {
			try {
				module.FS_unlink(assetPath);
			} catch {
				// Ignore missing file on first run.
			}
		}

		module.FS_createDataFile("/Content", asset, bytes, true, false, false);
	}
};

console.debug("initializing dotnet");
const runtime = await dotnet.withConfig({
}).create();

const config = runtime.getConfig();
const exports = await runtime.getAssemblyExports(config.mainAssemblyName);
const canvas = document.getElementById("canvas");
dotnet.instance.Module.canvas = canvas;

self.wasm = {
	Module: dotnet.instance.Module,
	dotnet,
	runtime,
	config,
	exports,
	canvas,
};

console.debug("PreInit...");
await runtime.runMain();
await exports.Program.PreInit();
await preloadContentAssets(dotnet.instance.Module);
console.debug("dotnet initialized");

console.debug("Init...");
await exports.Program.Init();

console.debug("MainLoop...");
const main = async () => {
	const ret = await exports.Program.MainLoop();

	if (!ret) {
		console.debug("Cleanup...");
		await exports.Program.Cleanup();
		return;
	}

	requestAnimationFrame(main);
}
requestAnimationFrame(main);
