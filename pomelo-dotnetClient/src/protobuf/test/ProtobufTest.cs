using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pomelo.Protobuf;

namespace Pomelo.Protobuf.Test
{
    public class ProtobufTest
    {
        // public static JObject read(string name)
        // {
        //     StreamReader file = new StreamReader(name);
        //
        //     String str = file.ReadToEnd();
        //
        //     return (JObject)SimpleJson.SimpleJson.DeserializeObject(str);
        // }

        public static bool equal(JObject a, JObject b)
        {
            // ICollection<string> keys0 = a.Keys;
            // ICollection<string> keys1 = b.Keys;
            //
            // foreach (string key in keys0)
            // {
            //     Console.WriteLine(a[key].GetType());
            //     if (a[key].GetType().ToString() == "SimpleJson.JObject")
            //     {
            //         if (!equal((JObject)a[key], (JObject)b[key])) return false;
            //     }
            //     else if (a[key].GetType().ToString() == "SimpleJson.JsonArray")
            //     {
            //         continue;
            //     }
            //     else
            //     {
            //         if (!a[key].ToString().Equals(b[key].ToString())) return false;
            //     }
            // }

            return true;
        }

        public static void Run()
        {
            // JObject protos = read("../../json/rootProtos.json");
            // JObject msgs = read("../../json/rootMsg.json");

            // Protobuf protobuf = new Protobuf(protos, protos);

            // for (int i = 0; i < msgs.Count; i++)
            // {
            //     JObject msg = (JObject)msgs[i];
            //     
            //     byte[] bytes = protobuf.encode(msgs[i].ToString(), msg[i]);
            //     JObject result = protobuf.decode(key, bytes);
            //     if (!equal(msg, result))
            //     {
            //         Console.WriteLine("protobuf test failed!");
            //         return;
            //     }
            // }
            //
            // ICollection<string> keys = msgs.Keys;
            //
            // foreach (string key in keys)
            // {
            //     JObject msg = (JObject)msgs[key];
            //     byte[] bytes = protobuf.encode(key, msg);
            //     JObject result = protobuf.decode(key, bytes);
            //     if (!equal(msg, result))
            //     {
            //         Console.WriteLine("protobuf test failed!");
            //         return;
            //     }
            // }
            //
            // Console.WriteLine("Protobuf test success!");
        }

        private static void print(byte[] bytes, int offset, int length)
        {
            for (int i = offset; i < length; i++)
                Console.Write(Convert.ToString(bytes[i], 16) + " ");
            Console.WriteLine();
        }
    }
}