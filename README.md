pomelo-unityclient-socket
=============================
这个是pomelo的 unity3d(.net)客户端，可在android,ios,pc上使用。
网易官方pomelo的.net客户端现在处于无人维护状态，好多issue也无人修改，提交了也没人合并，所以fork了一下，继续维护。

依赖的库:
* [SimpleJson](http://simplejson.codeplex.com/)：json解析和序列化的库(目前ios上解析List时候会有bug，尚未修正，后续版本会修正)。

## Demo
* [pomelo-chat-demo](https://github.com/koalaylj/pomelo-chat-demo)：demo for test，客户端用C#重写了下官方那个聊天的例子。

## How to use
	把 dist目录下 SimpleJson.dll 和 pomelo-dotnetClient.dll 这两个文件放到Unity3D项目 Assets/Plugins目录下。

## API

Create the connect

```c#
using namespace Pomelo.DotNetClient
string host = "127.0.0.1"; // or host="www.xxx.com"
int port = 3014;
PomeloClient pclient = new PomeloClient(host, port);

//The user data is the handshake user params
JsonObject user = new JsonObject();
pclient.connect(user, (data)=>{
  //process handshake call back data
  //not main thread.
});

```

Use request and response
```c#
pclient.request(route, message, (data)=>{
    // process the data
    // not main thread
});
```

Notify server without response

```c#
pclient.notify(route, messge);
```

Add event listener, process broadcast message
```c#
pclient.on(route, (data)=>{
    // process the data
    // not main thread
});
```
Disconnect the client.
```c#
pclient.disconnect();
```
## 已知BUG
* [can't parse json array on iOS](https://github.com/NetEase/pomelo-unityclient-socket/issues/10)
* socket断开处理细节处理
* 客户端消息缓冲池
* socket.connect需要改成异步处理
* 通知上层当前的网络状态

## Release Note
* 2014年2月9日
	- NewFeature : PomeloClient实现域名解析；
	- BugFix:socket意外断开不会调用OnDisconnect问题；




##License
(The MIT License)

Copyright (c) 2012-2013 NetEase, Inc. and other contributors

Permission is hereby granted, free of charge, to any person obtaining a
copy of this software and associated documentation files (the 'Software'),
to deal in the Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
