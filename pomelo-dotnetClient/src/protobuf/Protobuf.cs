using System;
using SimpleJson;

namespace Pomelo.Protobuf
{
    public class Protobuf
    {
        private MsgDecoder decoder;
        private MsgEncoder encoder;

        public Protobuf(JsonObject encodeProtos, JsonObject decodeProtos)
        {
            this.encoder = new MsgEncoder(encodeProtos);
            this.decoder = new MsgDecoder(decodeProtos);
        }

        public byte[] encode(string route, JsonObject msg)
        {
            return encoder.encode(route, msg);
        }

        public JsonObject decode(string route, byte[] buffer)
        {
            return decoder.decode(route, buffer);
        }
    }
}