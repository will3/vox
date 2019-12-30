const fs = require('fs-extra');
const Vox = require('./Vox');
const KaitaiStream = require('kaitai-struct/KaitaiStream');
const path = require('path');

const outPath = './Assets/Resources/models';
const inPath = './parse/in';
var fileNames = fs.readdirSync(inPath);

fs.emptyDirSync(outPath);

readIn();

function readIn() {
	for (var i in fileNames) {
		var fileName = fileNames[i];
		var regex = /^.*\.vox$/
		if (!regex.test(fileName)) {
			continue;
		}
		var name = fileName.substring(0, fileName.length - 4);
		readModel(name);
	}
}

function readChunk(childChunk, voxels, colors, size) {
	switch (childChunk.chunkId) {
		case Vox.ChunkType.SIZE:
			const chunkSize = childChunk.chunkContent;
			size[0] = chunkSize.sizeX;
			size[1] = chunkSize.sizeY;
			size[2] = chunkSize.sizeZ;
			break;
		case Vox.ChunkType.MATT:
			break;
		case Vox.ChunkType.XYZI:
			for (var i in childChunk.chunkContent.voxels) {
				var voxel = childChunk.chunkContent.voxels[i];
				if (voxel.colorIndex === 0) {
					throw new Error("Unexpected 0 color index");
				}
				voxels.push({
					X: voxel.x, 
					Z: voxel.y, 
					Y: voxel.z, 
					ColorIndex: voxel.colorIndex - 1
				});
			}
			break;
		case Vox.ChunkType.PACK:
			break;
		case Vox.ChunkType.RGBA:
			for (var i in childChunk.chunkContent.colors) {
				var color = childChunk.chunkContent.colors[i];
				colors.push({
					r: color.r / 255.0, 
					g: color.g / 255.0, 
					b: color.b / 255.0, 
					a: color.a / 255.0
				});
			}
			break;
	}
};

function readModel(name) {
	const fileContent = fs.readFileSync(path.join(inPath, name + '.vox'));
	const model = new Vox(new KaitaiStream(fileContent));

	var voxels = [];
	var colors = [];
	var size = [];
	for (var i in model.main.childrenChunks) {
		var childChunk = model.main.childrenChunks[i];

		readChunk(childChunk, voxels, colors, size);
	}

	var output = { Voxels: voxels, Palette: colors, Size: size };

	const outputPath = path.join(outPath, name) + '.json';
	fs.writeFileSync(outputPath, JSON.stringify(output));

	console.log(`Exported model ${outputPath}`);
};