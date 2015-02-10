#include "cpu_dump.h"
#include "cpu_profile.h"

namespace netease {
    Persistent<ObjectTemplate> CpuDump::_dump_template_;

    void CpuDump::Initialize(Handle<Object> target) {
        HandleScope scope;

        _dump_template_ = Persistent<ObjectTemplate>::New(ObjectTemplate::New());
        _dump_template_->SetInternalFieldCount(1);

        Local<Object> obj = _dump_template_->NewInstance();

        NODE_SET_METHOD(obj, "getProfile", CpuDump::GetProfile);
        NODE_SET_METHOD(obj, "start", CpuDump::StartProfiling);
        NODE_SET_METHOD(obj, "stop", CpuDump::StopProfiling);
        NODE_SET_METHOD(obj, "deleteAll", CpuDump::DeleteAllProfiles);

        target->Set(String::NewSymbol("cpu"), obj);
    }

    CpuDump::CpuDump() {}
    CpuDump::~CpuDump() {}


    Handle<Value> CpuDump::GetProfile(const Arguments& args) {
        HandleScope scope;
        if (args.Length() < 1) {
            return ThrowException(Exception::Error(String::New("No index specified")));
        } else if (!args[0]->IsInt32()) {
            return ThrowException(Exception::TypeError(String::New("Argument must be an integer")));
        }
        int32_t index = args[0]->Int32Value();
        const CpuProfile* profile = v8::CpuProfiler::GetProfile(index);
        return scope.Close(Profile::New(profile));
    }

    Handle<Value> CpuDump::StartProfiling(const Arguments& args) {
        HandleScope scope;
        Local<String> title = args.Length() > 0 ? args[0]->ToString() : String::New("");
        v8::CpuProfiler::StartProfiling(title);
        return Undefined();
    }

    Handle<Value> CpuDump::StopProfiling(const Arguments& args) {
        HandleScope scope;
        Local<String> title = args.Length() > 0 ? args[0]->ToString() : String::New("");
        const CpuProfile* profile = v8::CpuProfiler::StopProfiling(title);
        return scope.Close(Profile::New(profile));
    }

    Handle<Value> CpuDump::DeleteAllProfiles(const Arguments& args) {
        v8::CpuProfiler::DeleteAllProfiles();
        return Undefined();
    }

} 