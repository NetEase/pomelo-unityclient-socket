using Pomelo.DotNetClient;
using SimpleJson;
using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

public delegate void StringDelegate(string log);
public delegate void BoolDelegate(bool enabled);
public delegate string GetStringDelegate();

public delegate void VoidDelegate();

namespace Pomelo_NativeSocket
{

    public partial class Main : Form
    {

        private string _gate_server_ip = "127.0.0.1";
        private int _gate_server_port = 3014;

        public static JsonObject _users = null;
        public static PomeloClient _pomelo = null;
        private ArrayList _userList = new ArrayList();

        public Main()
        {
            InitializeComponent();
            AppendLog("Main Thread:" + Thread.CurrentThread.ManagedThreadId);
            _userList.Add("all");
            RefreshUserList();
            cb_users.SelectedIndex = 0;

            //Transporter.OnSocketClosedEvent += (message) =>
            //{
            //    AppendLog(message);
            //    SetEnabled(true);
            //};
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            sendMessage();
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            LoginGateServer(tb_name.Text);
        }


        private void SetEnabled(bool enabled)
        {
            if (btn_connect.InvokeRequired)
            {
                BoolDelegate d = SetEnabled;
                this.Invoke(d, enabled);
            }
            else
            {
                btn_connect.Enabled = enabled;
                tb_name.Enabled = enabled;
                tb_channel.Enabled = enabled;
                btn_send.Enabled = !enabled;
            }
        }

        private void RefreshUserList()
        {
            if (cb_users.InvokeRequired)
            {
                VoidDelegate d = RefreshUserList;
                this.Invoke(d);
            }
            else
            {
                cb_users.Items.Clear();
                cb_users.Items.AddRange(_userList.ToArray());
            }
        }

        private void AppendLog(string log)
        {
            if (tb_info.InvokeRequired)
            {
                StringDelegate d = AppendLog;
                this.Invoke(d, log);
                
            }
            else
            {
                tb_info.AppendText(log + "\n");
                tb_info.Focus();
                Console.WriteLine(log);
            }
        }


        /// <summary>
        /// 连接gate服务器
        /// </summary>
        /// <param name="userName"></param>
        void LoginGateServer(string userName)
        {
            _pomelo = new PomeloClient(_gate_server_ip, _gate_server_port);
            AppendLog("开始连接 gate server  " + _gate_server_ip + ":" + _gate_server_port);
            _pomelo.connect(null, (data) =>
            {
                AppendLog("成功连接 gate server ");
                JsonObject msg = new JsonObject();
                msg["uid"] = userName;

                _pomelo.request("gate.gateHandler.queryEntry", msg, LoginGateServerCallback);
            });
        }

        void LoginGateServerCallback(JsonObject result)
        {
            if (Convert.ToInt32(result["code"]) == 200)
            {

                _pomelo.disconnect();

                SetEnabled(false);

                LoginConnectorServer(result);
            }
            else
            {
                AppendLog("oh shit... Cannot access connector..");
            }
        }

        /// <summary>
        /// 连接connector服务器
        /// </summary>
        /// <param name="result"></param>
        void LoginConnectorServer(JsonObject result)
        {

            string host = (string)result["host"];
            int port = Convert.ToInt32(result["port"]);

            AppendLog("Connector Server 分配成功 " + host + ":" + port);

            _pomelo = new PomeloClient(host, port);

            _pomelo.connect(null, (data) =>
            {
                AppendLog("成功连接 connector server " + host + ":" + port);
                JoinChannel(tb_name.Text, tb_channel.Text);
            });
        }

        /// <summary>
        /// 加入频道
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="channel"></param>
        void JoinChannel(string userName, string channel)
        {
            JsonObject userMessage = new JsonObject();
            userMessage.Add("username", userName);
            userMessage.Add("rid", channel);

            if (_pomelo != null)
            {
                //请求加入聊天室
                _pomelo.request("connector.entryHandler.enter", userMessage, (data) =>
                {
                    _users = data;
                    if (_users != null)
                    {
                        AppendLog("进入 channel:" + _users.ToString());
                    }
                    else
                    {
                        AppendLog("进入 channel,but users == null");
                    }

                    InitChat();
                });
            }
        }

        void InitChat()
        {
            System.Object users = null;

            if (_users.TryGetValue("users", out users))
            {
                string u = users.ToString();
                string[] initUsers = u.Substring(1, u.Length - 2).Split(new Char[] { ',' });
                int length = initUsers.Length;
                for (int i = 0; i < length; i++)
                {
                    string s = initUsers[i];
                    _userList.Add(s.Substring(1, s.Length - 2));
                }
            }

            RefreshUserList();

            _pomelo.on("onAdd", (data) =>
            {
                AppendLog("onAdd:" + data);
                RefreshUserWindow("add", data);
            });

            _pomelo.on("onLeave", (data) =>
            {
                AppendLog("onLeave:" + data);
                RefreshUserWindow("leave", data);
            });

            _pomelo.on("onChat", (data) =>
            {
                AppendLog("onChat:" + data);
                addMessage(data);
            });

            _pomelo.on("disconnect", delegate(JsonObject msg)
            {
                AppendLog("disconnect : " + msg);
            });
        }

        void RefreshUserWindow(string flag, JsonObject msg)
        {
            System.Object user = null;
            if (msg.TryGetValue("user", out user))
            {
                if (flag == "add")
                {
                    this._userList.Add(user.ToString());
                }
                else if (flag == "leave")
                {
                    this._userList.Remove(user.ToString());
                }
            }
            RefreshUserList();
        }

        private ArrayList chatRecords = new ArrayList();

        void addMessage(JsonObject messge)
        {
            System.Object msg = null, fromName = null, targetName = null;
            if (messge.TryGetValue("msg", out msg) && messge.TryGetValue("from", out fromName) &&
                messge.TryGetValue("target", out targetName))
            {
                chatRecords.Add(new ChatRecord(fromName.ToString(), msg.ToString()));
            }
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        void sendMessage()
        {
            string inputField = tb_send.Text;

            if ("all".Equals(cb_users.Text))
            {
                chat("*", inputField);
            }
            else
            {
                solo();
            }
            tb_send.Clear();
        }

        //Chat with someone only.
        void solo()
        {
            chat(cb_users.Text, tb_send.Text);
            tb_send.Clear();
        }

        /// <summary>
        /// 发送聊天请求
        /// </summary>
        /// <param name="target"></param>
        /// <param name="content"></param>
        void chat(string target, string content)
        {
            string userName = tb_name.Text;
            JsonObject message = new JsonObject();
            message.Add("rid", tb_channel.Text);
            message.Add("content", content);
            message.Add("from", userName);
            message.Add("target", target);

            _pomelo.request("chat.chatHandler.send", message, (data) =>
            {
                if (target != "*" && target != userName)
                {
                    chatRecords.Add(new ChatRecord(userName, content));
                }
            });

            // _pomelo.notify("chat.chatHandler.send",message);
        }

        public static void kick()
        {
            _pomelo.request("connector.entryHandler.onUserLeave", delegate(JsonObject data)
            {
                Console.WriteLine("userLeave " + data);
            });
        }
    }
}
