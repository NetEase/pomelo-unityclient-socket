#ifndef NODE_HEAPDUMP_H_
#define NODE_HEAPDUMP_H_

#include <node.h>

using namespace v8;
using namespace node;

namespace netease 
{
	class HeapDump {
        public:
            static void Initialize(Handle<Object> target);

            HeapDump();
            virtual ~HeapDump();

        protected:
            static Handle<Value> WriteSnapshot(const Arguments& args);

        private:
            static Persistent<ObjectTemplate> _dump_template_;
    };

}

#endif // NODE_HEAPDUMP_H_
