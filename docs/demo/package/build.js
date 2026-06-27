import { copyFile } from "node:fs/promises";

console.log(JSON.stringify(process.env, Object.keys(process.env).sort(), 2));

// Just emulate a build process...
await copyFile("Client/index.js", process.env.OUT_DIR + "index.min.js");
