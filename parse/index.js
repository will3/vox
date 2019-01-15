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

function readChunk(childChunk, voxels, colors) {
	var size;
	switch (childChunk.chunkId) {
		case Vox.ChunkType.SIZE:
			size = childChunk.chunkContent;
			break;
		case Vox.ChunkType.MATT:
			// console.log(childChunk.chunkContent);
			break;
		case Vox.ChunkType.XYZI:
			for (var i in childChunk.chunkContent.voxels) {
				var voxel = childChunk.chunkContent.voxels[i];
				voxels.push({
					x: voxel.x, 
					y: voxel.y, 
					z: voxel.z, 
					colorIndex: voxel.colorIndex
				});
			}
			break;
		case Vox.ChunkType.PACK:
			// console.log(childChunk.chunkContent);
			break;
		case Vox.ChunkType.RGBA:
			for (var i in childChunk.chunkContent.colors) {
				var color = childChunk.chunkContent.colors[i];
				colors.push({
					r: color.r, 
					g: color.g, 
					b: color.b, 
					a: color.a
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
	for (var i in model.main.childrenChunks) {
		var childChunk = model.main.childrenChunks[i];

		readChunk(childChunk, voxels, colors);
	}

	var output = { voxels, colors };

	fs.writeFileSync(path.join(outPath, name), JSON.stringify(output));
};