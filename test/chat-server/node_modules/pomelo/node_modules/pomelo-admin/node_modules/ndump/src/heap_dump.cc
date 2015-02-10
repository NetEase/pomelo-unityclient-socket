/*
 * Copyright (c) 2012, Ben Noordhuis <info@bnoordhuis.nl>
 *
 * Permission to use, copy, modify, and/or distribute this software for any
 * purpose with or without fee is hereby granted, provided that the above
 * copyright notice and this permission notice appear in all copies.
 *
 * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
 * WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
 * ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
 * WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
 * ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
 * OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
 */

#include "heap_dump.h"
#include "util.h"
 #include "file_output.h"

#include "node.h"
#include "uv.h"
#include "v8.h"
#include "v8-profiler.h"


#include <stdio.h>
#include <stdarg.h>
#include <stdlib.h>
#include <assert.h>

using v8::Arguments;
using v8::FunctionTemplate;
using v8::Handle;
using v8::HandleScope;
using v8::HeapProfiler;
using v8::HeapSnapshot;
using v8::HeapStatistics;
using v8::Object;
using v8::OutputStream;
using v8::String;
using v8::Undefined;
using v8::V8;
using v8::Value;
using namespace v8;
using namespace node;

namespace netease {

   Persistent<ObjectTemplate> HeapDump::_dump_template_;

    void HeapDump::Initialize(Handle<Object> target) {
        HandleScope scope;
        _dump_template_ = Persistent<ObjectTemplate>::New(ObjectTemplate::New());
        _dump_template_->SetInternalFieldCount(1);

        Local<Object> obj = _dump_template_->NewInstance();

        NODE_SET_METHOD(obj, "writeSnapshot", HeapDump::WriteSnapshot);

        target->Set(String::NewSymbol("heap"), obj);
    }

    HeapDump::HeapDump() {}
    HeapDump::~HeapDump() {}

    Handle<Value> HeapDump::WriteSnapshot(const Arguments& args) {
      if (args.Length() < 1) {
          return ThrowException(Exception::Error(String::New("No index specified")));
        } 
        if (!args[0]->IsString()) {
          return ThrowException(Exception::TypeError(String::New("Argument[0] must be an string")));
        } 
        Local<String> filename = args.Length() > 0 ? args[0]->ToString() : String::New("");

        FILE* fp = fopen(netease::get(filename), "w");
        if (fp == NULL) return Undefined();
        const HeapSnapshot* snap = HeapProfiler::TakeSnapshot(String::Empty());
        FileOutputStream stream(fp);
        snap->Serialize(&stream, HeapSnapshot::kJSON);
        fclose(fp);
        return Undefined();
    }
 }