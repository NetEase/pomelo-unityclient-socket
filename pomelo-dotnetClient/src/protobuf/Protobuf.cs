using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pomelo.Protobuf
{
    public class Protobuf
    {
        private MsgDecoder decoder;
        private MsgEncoder encoder;

        public Protobuf(JObject encodeProtos, JObject decodeProtos)
        {
            this.encoder = new MsgEncoder(encodeProtos);
            this.decoder = new MsgDecoder(decodeProtos);
        }

        public byte[] encode(string route, JObject msg)
        {
            return encoder.encode(route, msg);
        }

        public JObject decode(string route, byte[] buffer)
        {
            return decoder.decode(route, buffer);
        }
    }
}