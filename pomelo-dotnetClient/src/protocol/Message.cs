using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pomelo.DotNetClient
{
    public class Message
    {
        public MessageType type;
        public string route;
        public uint id;
        public JObject data;

        public Message(MessageType type, uint id, string route, JObject data)
        {
            this.type = type;
            this.id = id;
            this.route = route;
            this.data = data;
        }
    }
}