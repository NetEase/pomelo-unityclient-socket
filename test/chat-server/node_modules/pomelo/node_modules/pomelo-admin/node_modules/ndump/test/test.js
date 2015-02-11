var path = require('path');
var h = require(path.join(__dirname, '/../build/Release/ndump'));
console.log(h);
h.heap.writeSnapshot(__dirname+"/log.file");
h.cpu.start('fff');
console.log(h.cpu.stop('fff'));
