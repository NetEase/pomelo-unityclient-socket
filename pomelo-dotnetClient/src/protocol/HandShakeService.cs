using System;
using System.Text;
using SimpleJson;
using System.Net;
using System.Net.Sockets;

namespace Pomelo.DotNetClient
{
    public class HandShakeService
    {
        private Protocol protocol;
        private Action<JsonObject> callback;

        public const string Version = "0.3.0";
        public const string Type = "unity-socket";


        public HandShakeService(Protocol protocol)
        {
            this.protocol = protocol;
        }

        public void request(JsonObject user, Action<JsonObject> callback)
        {
            byte[] body = Encoding.UTF8.GetBytes(buildMsg(user).ToString());

            protocol.send(PackageType.PKG_HANDSHAKE, body);

            this.callback = callback;
        }

        internal void invokeCallback(JsonObject data)
        {
            //Invoke the handshake callback
            if (callback != null) callback.Invoke(data);
        }

        public void ack()
        {
            protocol.send(PackageType.PKG_HANDSHAKE_ACK, new byte[0]);
        }

        private JsonObject buildMsg(JsonObject user)
        {
            if (user == null) user = new JsonObject();

            JsonObject msg = new JsonObject();

            //Build sys option
            JsonObject sys = new JsonObject();
            sys["version"] = Version;
            sys["type"] = Type;

            //Build handshake message
            msg["sys"] = sys;
            msg["user"] = user;

            return msg;
        }
    }
}