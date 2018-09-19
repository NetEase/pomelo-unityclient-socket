using System;
using Pomelo.Protobuf.Test;

namespace Pomelo.DotNetClient.Test
{
    public class Test
    {
        public static void Main(string[] args)
        {
            byte[] bytes = Pomelo.Protobuf.Encoder.encodeUInt32(112321);
            Console.WriteLine(Pomelo.Protobuf.Decoder.decodeUInt32(bytes));
            CodecTest.Run();
            ProtobufTest.Run();
            TransportTest.Run();
            ClientTest.Run();
            Console.ReadKey();
        }
    }
}