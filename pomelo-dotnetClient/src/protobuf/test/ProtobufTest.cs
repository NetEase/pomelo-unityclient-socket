using System;
using System.Collections.Generic;
using System.IO;
using SimpleJson;
using Pomelo.Protobuf;

namespace Pomelo.Protobuf.Test
{
    public class ProtobufTest
    {
        public static JsonObject read(string name)
        {
            StreamReader file = new StreamReader(name);

            String str = file.ReadToEnd();

            return (JsonObject)SimpleJson.SimpleJson.DeserializeObject(str);
        }

        public static bool equal(JsonObject a, JsonObject b)
        {
            ICollection<string> keys0 = a.Keys;
            ICollection<string> keys1 = b.Keys;

            foreach (string key in keys0)
            {
                Console.WriteLine(a[key].GetType());
                if (a[key].GetType().ToString() == "SimpleJson.JsonObject")
                {
                    if (!equal((JsonObject)a[key], (JsonObject)b[key])) return false;
                }
                else if (a[key].GetType().ToString() == "SimpleJson.JsonArray")
                {
                    continue;
                }
                else
                {
                    if (!a[key].ToString().Equals(b[key].ToString())) return false;
                }
            }

            return true;
        }

        public static void Run()
        {
            JsonObject protos = read("../../json/rootProtos.json");
            JsonObject msgs = read("../../json/rootMsg.json");

            Protobuf protobuf = new Protobuf(protos, protos);

            ICollection<string> keys = msgs.Keys;

            foreach (string key in keys)
            {
                JsonObject msg = (JsonObject)msgs[key];
                byte[] bytes = protobuf.encode(key, msg);
                JsonObject result = protobuf.decode(key, bytes);
                if (!equal(msg, result))
                {
                    Console.WriteLine("protobuf test failed!");
                    return;
                }
            }

            Console.WriteLine("Protobuf test success!");
        }

        private static void print(byte[] bytes, int offset, int length)
        {
            for (int i = offset; i < length; i++)
                Console.Write(Convert.ToString(bytes[i], 16) + " ");
            Console.WriteLine();
        }
    }
}