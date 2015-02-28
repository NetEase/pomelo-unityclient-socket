pomelo-unityclient-socket
=============================
This is the pomelo dotnet client, support pomelo 0.3 and the new communicate protocol.It is based on native socket.
The project is based on some libraries as follows:

* [simple-json](https://github.com/facebook-csharp-sdk/simple-json) An open source json library

## Demo

* [Unity3D demo](https://github.com/NetEase/pomelo-unitychat-socket) A pomelo-chat client use unity 3D.
* [dotnet demo](https://github.com/NetEase/pomelo-dotnetchat-console) A pomelo-chat client use console and write by c#.

## How to use
To use the dotnetClient, just include ./dist/*.dll in your project.

## API

Initialize pomelo client

```c#
using namespace Pomelo.DotNetClient
string host="127.0.0.1";//(www.xxx.com/127.0.0.1/::1/localhost etc.)
int port=3014;
PomeloClient pclient = new PomeloClient();

//listen on network state changed event
pclient.NetWorkStateChangedEvent += (state) =>
{
    Console.WriteLine(state);
};

pclient.initClient(host, port, () =>
{
    //The user data is the handshake user params
    JsonObject user = new JsonObject();
    pclient.connect(user, data =>
    {
     	//process handshake call back data
    });
});

```

Use request and response
```c#
pclient.request(route, message, (data)=>{
    //process the data
});
```

Notify server without response

```c#
pclient.notify(route, messge);
```

Add event listener, process broadcast message
```c#
pclient.on(route, (data)=>{
    //process the data
});
```
Disconnect the client.
```c#
pclient.disconnect();
```

protobuf surpported type:`uInt32, int32, sInt32, float, double, string.`

The dotnet client support dictionary compress and protbuf encode. The use these function, just set the flag at server side.
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
