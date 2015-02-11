var fs = require('fs');

var _ndump = require('./build/Release/ndump');

ndump = {};

ndump.heap = function(file){
	_ndump.heap.writeSnapshot(file);
}

ndump.cpu = function(file,times){
	_ndump.cpu.start(file);
	setTimeout(function(){
		fs.writeFileSync(file,JSON.stringify(_ndump.cpu.stop(file)));
		_ndump.cpu.deleteAll();
	},times)
}

module.exports = ndump;