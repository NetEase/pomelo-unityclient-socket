using SimpleJson;

using System;
using System.Net;
using System.Net.Sockets;

namespace Pomelo.DotNetClient
{
    public class PomeloClient : IDisposable
    {
        public const string EVENT_DISCONNECT = "disconnect";

        private EventManager eventManager;
        private Socket socket;
        private Protocol protocol;
        private bool disposed = false;
        private uint reqId = 1;

        /// <summary>
        /// 客户端初始化
        /// </summary>
        /// <param name="host">server name or server ip (支持www.xxx.com/127.0.0.1/::1/localhost)</param>
        /// <param name="port">server port</param>
        public PomeloClient(string host, int port)
        {
            this.eventManager = new EventManager();
            initClient(host, port);
            this.protocol = new Protocol(this, socket);
        }

        /// <summary>
        /// 初始化客户端socket连接
        /// </summary>
        /// <param name="host">server name or server ip (支持www.xxx.com/127.0.0.1/::1/localhost)</param>
        /// <param name="port">server port</param>
        private void initClient(string host, int port)
        {
            IPAddress ipAddress = null;

            IPAddress[] addresses = Dns.GetHostEntry(host).AddressList;

            foreach (var item in addresses)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = item;
                    break;
                }
            }

            if (ipAddress == null)
            {
                throw new Exception("can not parse host : " + host);
            }

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ie = new IPEndPoint(ipAddress, port);

            try
            {
                this.socket.Connect(ie);
            }
            catch (SocketException e)
            {
                Console.WriteLine(String.Format("unable to connect to server: {0}", e.ToString()));
                return;
            }
        }

        public void connect()
        {
            connect(null, null);
        }

        public void connect(JsonObject user)
        {
            connect(user, null);
        }

        public void connect(Action<JsonObject> handshakeCallback)
        {
            connect(null, handshakeCallback);
        }

        public bool connect(JsonObject user, Action<JsonObject> handshakeCallback)
        {
            try
            {
                protocol.start(user, handshakeCallback);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        private JsonObject emptyMsg = new JsonObject();
        public void request(string route, Action<JsonObject> action)
        {
            this.request(route, emptyMsg, action);
        }

        public void request(string route, JsonObject msg, Action<JsonObject> action)
        {
            this.eventManager.AddCallBack(reqId, action);
            protocol.send(route, reqId, msg);

            reqId++;
        }

        public void notify(string route, JsonObject msg)
        {
            protocol.send(route, msg);
        }

        public void on(string eventName, Action<JsonObject> action)
        {
            eventManager.AddOnEvent(eventName, action);
        }

        internal void processMessage(Message msg)
        {
            if (msg.type == MessageType.MSG_RESPONSE)
            {
                eventManager.InvokeCallBack(msg.id, msg.data);
            }
            else if (msg.type == MessageType.MSG_PUSH)
            {
                eventManager.InvokeOnEvent(msg.route, msg.data);
            }
        }

        public void disconnect()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code 
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;

            if (disposing)
            {
                // free managed resources
                this.protocol.close();

                try
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception)
                {
                    //todo : 有待确定这里是否会出现异常，这里是参考之前官方github上pull request。emptyMsg
                }

                this.socket.Close();
                this.disposed = true;
                //Call disconnect callback
                eventManager.InvokeOnEvent(EVENT_DISCONNECT, null);
            }
        }
        /// <summary>
        /// 主线程调用，获取接受消息的更新
        /// </summary>
        public void UpdateRevice()
        {
            if(this.protocol != null)
            {
                this.protocol.Update();
            }
        }
    }
}

