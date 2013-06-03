using System;
using SimpleJson;

namespace Pomelo.DotNetClient.Test
{
	public class ClientTest
	{
		public static PomeloClient pc = null;

		public static void loginTest(string host, int port){
			pc = new PomeloClient(host, port);

			pc.connect(null, data=>{
				Console.WriteLine("on data back" + data.ToString());
				JsonObject msg  = new JsonObject();
				msg["uid"] = 111;
				pc.request("gate.gateHandler.queryEntry", msg, OnQuery);
			});

			while(true){
				System.Threading.Thread.Sleep(100);
			};
		}

		public static void OnQuery(JsonObject result){
			if(Convert.ToInt32(result["code"]) == 200){
				Console.WriteLine("dis connect");
				pc.disconnect();
				
				string host = (string)result["host"];
				int port = Convert.ToInt32(result["port"]);
				pc = new PomeloClient(host, port);
				pc.connect(null, (data)=>{
					JsonObject userMessage = new JsonObject();
					Console.WriteLine("on connect to connector!");
				});	
			}
		}

		public static void Run(){
			string host = "127.0.0.1";
			int port = 3014;

			loginTest(host, port);
		}
	}
}

