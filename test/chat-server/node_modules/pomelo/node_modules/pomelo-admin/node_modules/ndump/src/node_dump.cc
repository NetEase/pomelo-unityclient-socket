#ifndef NODE_DUMP_
#define NODE_DUMP_

#include <v8-profiler.h>
#include "heap_dump.h"
#include "cpu_dump.h"
#include <stdio.h>
#include <stdarg.h>
#include <stdlib.h>
#include <assert.h>


using namespace v8;

namespace netease {

	static void initNodeDump(Handle<Object> target) {
		HandleScope scope;
		HeapDump::Initialize(target);
		CpuDump::Initialize(target);
	}

	NODE_MODULE(ndump,initNodeDump)

} 
#endif  // NODE_DUMP_
