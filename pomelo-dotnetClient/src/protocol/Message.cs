using System;
using SimpleJson;

namespace Pomelo.DotNetClient
{
    public class Message
    {
        public MessageType type;
        public string route;
        public uint id;
        public JsonObject data;

        public Message(MessageType type, uint id, string route, JsonObject data)
        {
            this.type = type;
            this.id = id;
            this.route = route;
            this.data = data;
        }
    }
}