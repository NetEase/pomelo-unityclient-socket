using System; 
using SimpleJson;
using System.Timers;
using Pomelo.Protobuf;
using Pomelo.DotNetClient;

namespace Pomelo.DotNetClient.Chat
{
	class Test
	{
		static PomeloClient pc;

		public static void Main(){
			Console.WriteLine ("before init");
			
			pc = new PomeloClient("127.0.0.1", 3014);
			
			Console.WriteLine ("before connect");
			pc.connect(null, delegate(JsonObject data){
				Console.WriteLine ("after connect " + data.ToString());
				JsonObject msg = new JsonObject();

				msg["uid"] = 1;
				pc.request("gate.gateHandler.queryEntry", msg, onQuery);
			});
			 
			while(true) {
				string str = Console.ReadLine();
				if(str != null){
					send (str);
				}
				System.Threading.Thread.Sleep(100);
			}
		}

		public static void onQuery(JsonObject result){
			if(Convert.ToInt32(result["code"]) == 200){
				string host = (string)result["host"];
				int port = Convert.ToInt32(result["port"]);
				pc.disconnect();

				startConnect(host, port);

			}
		}

		private static void startConnect(string host, int port){
			pc = new PomeloClient(host, port);
			pc.connect(null, delegate(JsonObject data){
				pc.on("onChat", delegate(JsonObject msg) {
					Console.WriteLine(msg["from"] + " : " + msg["msg"]);
				});
				pc.on("onAdd", delegate(JsonObject msg) {
					Console.WriteLine("onAdd : " + msg);
				});
				pc.on("onLeave", delegate(JsonObject msg) {
					Console.WriteLine("onLeave : " + msg);
				});
				pc.on("disconnect", delegate(JsonObject msg) {
					Console.WriteLine("disconnect : " + msg);
				});
				
				Console.WriteLine("connect to connector " + data.ToString());
				//kick ();
				login ();
			});	
		}

		public static void kick(){
			pc.request("connector.entryHandler.onUserLeave", delegate (JsonObject data) {
				Console.WriteLine("userLeave " + data);
			});
		}

		public static void login(){
			JsonObject args = new JsonObject();
			args["username"] = "zxg" + DateTime.Now.Millisecond;
			args["rid"] = "public";
			pc.request("connector.entryHandler.enter", args, delegate (JsonObject data) {
				Console.WriteLine("Login " + data);
			});
		}

		public static void send(string message){
			JsonObject args = new JsonObject();
			args["content"] = message;
			args["target"] = "*";

			pc.notify("chat.chatHandler.send", args);
		}
	}
}
