using System;
using Pomelo.DotNetClient.Test;
using Pomelo.Protobuf.Test;

namespace Pomelo.DotNetClient.Test
{
	public class Test
	{
		public static void Main()
		{
			CodecTest.Run();
			ProtobufTest.Run();
			TransportTest.Run();
		}
	}
}

