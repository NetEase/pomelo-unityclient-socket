#ifndef NODE_CPU_DUMP_H
#define NODE_CPU_DUMP_H

#include <v8-profiler.h>
#include <node.h>

using namespace v8;
using namespace node;

namespace netease {
    class CpuDump {
        public:
            static void Initialize(Handle<Object> target);

            CpuDump();
            virtual ~CpuDump();

        protected:
            static Handle<Value> DeleteAllProfiles(const Arguments& args);
            static Handle<Value> GetProfile(const Arguments& args);
            static Handle<Value> StartProfiling(const Arguments& args);
            static Handle<Value> StopProfiling(const Arguments& args);

        private:
            static Persistent<ObjectTemplate> _dump_template_;
    };
} //namespace netease

#endif  // NODE_CPU_DUMP_H
