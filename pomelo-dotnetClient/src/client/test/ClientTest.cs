using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


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
                    
                    JObject msg = new JObject();
                    msg["uid"] = 111;
                    pc.request("gate.gateHandler.queryEntry", msg, OnQuery);
                });
            });
        }

        public static void OnQuery(JObject result)
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
                        JObject userMessage = new JObject();
                        Console.WriteLine("on connect to connector!");

                        //Login
                        JObject msg = new JObject();
                        msg["username"] = "test";
                        msg["rid"] = "pomelo";

                        pc.request("connector.entryHandler.enter", msg, OnEnter);
                    });
                });
            }
        }

        public static void OnEnter(JObject result)
        {
            Console.WriteLine("on login " + result.ToString());
        }

        public static void onDisconnect(JObject result)
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