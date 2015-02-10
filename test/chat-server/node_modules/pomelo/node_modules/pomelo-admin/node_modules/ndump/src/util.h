
#ifndef NODE_PROFILE_UTIL_H_

#define NODE_PROFILE_UTIL_H_

#include <node.h>
#include <stdlib.h>
#include <cstring>

using namespace std;
using namespace v8;
using namespace node;

namespace netease {

  char *get(v8::Local<v8::Value> value, const char *fallback = "") {
    if (value->IsString()) {
      v8::String::AsciiValue string(value);
      char *str = (char *) malloc(string.length() + 1);
      strcpy(str, *string);
      return str;
    }
    char *str = (char *) malloc(strlen(fallback) + 1);
    strcpy(str, fallback);
    return str;
  }

}
  #endif // NODE_PROFILE_UTIL_H_