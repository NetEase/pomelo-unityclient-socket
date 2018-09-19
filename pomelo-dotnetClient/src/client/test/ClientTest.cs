using System;
using SimpleJson;

namespace Pomelo.DotNetClient.Test
{
    public class ClientTest
    {
        public static PomeloClient pc = null;

        public static void loginTest(string host, int port)
        {
            pc = new PomeloClient();

            pc.NetWorkStateChangedEvent += (state) =>
            {
                Console.WriteLine(state);
            };


            pc.initClient(host, port, () =>
            {
                pc.connect(null, data =>
                {

                    Console.WriteLine("on data back" + data.ToString());
                    JsonObject msg = new JsonObject();
                    msg["uid"] = 111;
                    pc.request("gate.gateHandler.queryEntry", msg, OnQuery);
                });
            });
        }

        public static void OnQuery(JsonObject result)
        {
            if (Convert.ToInt32(result["code"]) == 200)
            {
                pc.disconnect();

                string host = (string)result["host"];
                int port = Convert.ToInt32(result["port"]);
                pc = new PomeloClient();

                pc.NetWorkStateChangedEvent += (state) =>
                {
                    Console.WriteLine(state);
                };

                pc.initClient(host, port, () =>
                {
                    pc.connect(null, (data) =>
                    {
                        JsonObject userMessage = new JsonObject();
                        Console.WriteLine("on connect to connector!");

                        //Login
                        JsonObject msg = new JsonObject();
                        msg["username"] = "test";
                        msg["rid"] = "pomelo";

                        pc.request("connector.entryHandler.enter", msg, OnEnter);
                    });
                });
            }
        }

        public static void OnEnter(JsonObject result)
        {
            Console.WriteLine("on login " + result.ToString());
        }

        public static void onDisconnect(JsonObject result)
        {
            Console.WriteLine("on sockect disconnected!");
        }

        public static void Run()
        {
            string host = "192.168.0.156";
            int port = 3014;

            loginTest(host, port);
        }
    }
}