using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Pomelo.Protobuf
{
    public class MsgDecoder
    {
        private JObject protos { set; get; }//The message format(like .proto file)
        private int offset { set; get; }
        private byte[] buffer { set; get; }//The binary message from server.
        private Util util { set; get; }

        public MsgDecoder(JObject protos)
        {
            if (protos == null) protos = new JObject();

            this.protos = protos;
            this.util = new Util();
        }

        /// <summary>
        /// Decode message from server.
        /// </summary>
        /// <param name='route'>
        /// Route.
        /// </param>
        /// <param name='buf'>
        /// JObject.
        /// </param>
        public JObject decode(string route, byte[] buf)
        {
            this.buffer = buf;
            this.offset = 0;
            JToken proto = null;
            if (this.protos.TryGetValue(route, out proto))
            {
                JObject msg = new JObject();
                return this.decodeMsg(msg, (JObject)proto, this.buffer.Length);
            }
            return null;
        }


        /// <summary>
        /// Decode the message.
        /// </summary>
        /// <returns>
        /// The message.
        /// </returns>
        /// <param name='msg'>
        /// JObject.
        /// </param>
        /// <param name='proto'>
        /// JObject.
        /// </param>
        /// <param name='length'>
        /// int.
        /// </param>
        private JObject decodeMsg(JObject msg, JObject proto, int length)
        {
            while (this.offset < length)
            {
                Dictionary<string, int> head = this.getHead();
                int tag;
                if (head.TryGetValue("tag", out tag))
                {
                    JToken _tags = null;
                    if (proto.TryGetValue("__tags", out _tags))
                    {
                        JToken name;
                        if (((JObject)_tags).TryGetValue(tag.ToString(), out name))
                        {
                            JToken value;
                            if (proto.TryGetValue(name.ToString(), out value))
                            {
                                JToken option;
                                if (((JObject)(value)).TryGetValue("option", out option))
                                {
                                    switch (option.ToString())
                                    {
                                        case "optional":
                                        case "required":
                                            JToken type;
                                            if (((JObject)(value)).TryGetValue("type", out type))
                                            {
                                                msg.Add(name.ToString(), proto);
                                            }
                                            break;
                                        case "repeated":
                                            JToken _name;
                                            if (!msg.TryGetValue(name.ToString(), out _name))
                                            {
                                                msg.Add(name.ToString(), new JArray());
                                            }
                                            JToken value_type;
                                            if (msg.TryGetValue(name.ToString(), out _name) && ((JObject)(value)).TryGetValue("type", out value_type))
                                            {
                                                decodeArray((JArray)_name, value_type.ToString(), proto);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return msg;
        }

        /// <summary>
        /// Decode array in message.
        /// </summary>
        private void decodeArray(JArray list, string type, JObject proto)
        {
            if (this.util.isSimpleType(type))
            {
                int length = (int)Decoder.decodeUInt32(this.getBytes());
                for (int i = 0; i < length; i++)
                {
                    list.Add(this.decodeProp(type, null));
                }
            }
            else
            {
                list.Add(this.decodeProp(type, proto));
            }
        }

        /// <summary>
        /// Decode each simple type in message.
        /// </summary>
        private object decodeProp(string type, JObject proto)
        {
            switch (type)
            {
                case "uInt32":
                    return Decoder.decodeUInt32(this.getBytes());
                case "int32":
                case "sInt32":
                    return Decoder.decodeSInt32(this.getBytes());
                case "float":
                    return this.decodeFloat();
                case "double":
                    return this.decodeDouble();
                case "string":
                    return this.decodeString();
                default:
                    return this.decodeObject(type, proto);
            }
        }

        //Decode the user-defined object type in message.
        private JObject decodeObject(string type, JObject proto)
        {
            if (proto != null)
            {
                JToken __messages;
                if (proto.TryGetValue("__messages", out __messages))
                {
                    JToken _type;
                    if (((JObject)__messages).TryGetValue(type, out _type) || protos.TryGetValue("message " + type, out _type))
                    {
                        int l = (int)Decoder.decodeUInt32(this.getBytes());
                        JObject msg = new JObject();
                        return this.decodeMsg(msg, (JObject)_type, this.offset + l);
                    }
                }
            }
            return new JObject();
        }

        //Decode string type.
        private string decodeString()
        {
            int length = (int)Decoder.decodeUInt32(this.getBytes());
            string msg_string = Encoding.UTF8.GetString(this.buffer, this.offset, length);
            this.offset += length;
            return msg_string;
        }

        //Decode double type.
        private double decodeDouble()
        {
            double msg_double = BitConverter.Int64BitsToDouble((long)this.ReadRawLittleEndian64());
            this.offset += 8;
            return msg_double;
        }

        //Decode float type
        private float decodeFloat()
        {
            float msg_float = BitConverter.ToSingle(this.buffer, this.offset);
            this.offset += 4;
            return msg_float;
        }

        //Read long in littleEndian
        private ulong ReadRawLittleEndian64()
        {
            ulong b1 = buffer[this.offset];
            ulong b2 = buffer[this.offset + 1];
            ulong b3 = buffer[this.offset + 2];
            ulong b4 = buffer[this.offset + 3];
            ulong b5 = buffer[this.offset + 4];
            ulong b6 = buffer[this.offset + 5];
            ulong b7 = buffer[this.offset + 6];
            ulong b8 = buffer[this.offset + 7];
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24)
                  | (b5 << 32) | (b6 << 40) | (b7 << 48) | (b8 << 56);
        }

        //Get the type and tag.
        private Dictionary<string, int> getHead()
        {
            int tag = (int)Decoder.decodeUInt32(this.getBytes());
            Dictionary<string, int> head = new Dictionary<string, int>();
            head.Add("type", tag & 0x7);
            head.Add("tag", tag >> 3);
            return head;
        }

        //Get bytes.
        private byte[] getBytes()
        {
            List<byte> arrayList = new List<byte>();
            int pos = this.offset;
            byte b;
            do
            {
                b = this.buffer[pos];
                arrayList.Add(b);
                pos++;
            } while (b >= 128);
            this.offset = pos;
            int length = arrayList.Count;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = arrayList[i];
            }
            return bytes;
        }
    }
}