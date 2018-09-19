using System;
using System.Text;
using SimpleJson;
using System.Collections;
using System.Collections.Generic;

namespace Pomelo.Protobuf
{
    public class MsgDecoder
    {
        private JsonObject protos { set; get; }//The message format(like .proto file)
        private int offset { set; get; }
        private byte[] buffer { set; get; }//The binary message from server.
        private Util util { set; get; }

        public MsgDecoder(JsonObject protos)
        {
            if (protos == null) protos = new JsonObject();

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
        /// JsonObject.
        /// </param>
        public JsonObject decode(string route, byte[] buf)
        {
            this.buffer = buf;
            this.offset = 0;
            object proto = null;
            if (this.protos.TryGetValue(route, out proto))
            {
                JsonObject msg = new JsonObject();
                return this.decodeMsg(msg, (JsonObject)proto, this.buffer.Length);
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
        /// JsonObject.
        /// </param>
        /// <param name='proto'>
        /// JsonObject.
        /// </param>
        /// <param name='length'>
        /// int.
        /// </param>
        private JsonObject decodeMsg(JsonObject msg, JsonObject proto, int length)
        {
            while (this.offset < length)
            {
                Dictionary<string, int> head = this.getHead();
                int tag;
                if (head.TryGetValue("tag", out tag))
                {
                    object _tags = null;
                    if (proto.TryGetValue("__tags", out _tags))
                    {
                        object name;
                        if (((JsonObject)_tags).TryGetValue(tag.ToString(), out name))
                        {
                            object value;
                            if (proto.TryGetValue(name.ToString(), out value))
                            {
                                object option;
                                if (((JsonObject)(value)).TryGetValue("option", out option))
                                {
                                    switch (option.ToString())
                                    {
                                        case "optional":
                                        case "required":
                                            object type;
                                            if (((JsonObject)(value)).TryGetValue("type", out type))
                                            {
                                                msg.Add(name.ToString(), this.decodeProp(type.ToString(), proto));
                                            }
                                            break;
                                        case "repeated":
                                            object _name;
                                            if (!msg.TryGetValue(name.ToString(), out _name))
                                            {
                                                msg.Add(name.ToString(), new List<object>());
                                            }
                                            object value_type;
                                            if (msg.TryGetValue(name.ToString(), out _name) && ((JsonObject)(value)).TryGetValue("type", out value_type))
                                            {
                                                decodeArray((List<object>)_name, value_type.ToString(), proto);
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
        private void decodeArray(List<object> list, string type, JsonObject proto)
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
        private object decodeProp(string type, JsonObject proto)
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
        private JsonObject decodeObject(string type, JsonObject proto)
        {
            if (proto != null)
            {
                object __messages;
                if (proto.TryGetValue("__messages", out __messages))
                {
                    object _type;
                    if (((JsonObject)__messages).TryGetValue(type, out _type) || protos.TryGetValue("message " + type, out _type))
                    {
                        int l = (int)Decoder.decodeUInt32(this.getBytes());
                        JsonObject msg = new JsonObject();
                        return this.decodeMsg(msg, (JsonObject)_type, this.offset + l);
                    }
                }
            }
            return new JsonObject();
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