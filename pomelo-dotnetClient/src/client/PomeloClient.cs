//#define LUZEXI

using System.Collections;
using SimpleJson;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

//#if LUZEXI
////using System.Collections.Concurrent;
//#endif

namespace Pomelo.DotNetClient
{
	public class PomeloClient : IDisposable
	{
		public const string EVENT_DISCONNECT = "disconnect";

		private EventManager eventManager;
		private Socket socket;
		private Protocol protocol;
		private bool disposed = false;
		private uint reqId = 1;

#if LUZEXI
		//private ConcurrentQueue<Message> m_seqReceiveMessage = new ConcurrentQueue<Message>();	//The queue of the message receive
		private Queue<Message> m_seqReceiveMessage = new Queue<Message>();
		private System.Object m_cLock = new System.Object();	//the lock of the queue
		private const int PROCESS_NUM = 5;	//the process of the update num
#endif

		public PomeloClient(string host, int port) {
			this.eventManager = new EventManager();
			initClient(host, port);
			this.protocol = new Protocol(this, socket);
		}

		private void initClient(string host, int port) {
			this.socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
	        IPEndPoint ie=new IPEndPoint(IPAddress.Parse(host), port);
	        try {
	            this.socket.Connect(ie);
	        }
	        catch (SocketException e) {
	            Console.WriteLine(String.Format("unable to connect to server: {0}", e.ToString()));
	            return;
	        }
		}
		
		public void connect(){
			protocol.start(null, null);
		}
		
		public void connect(JsonObject user){
			protocol.start(user, null);
		}
		
		public void connect(Action<JsonObject> handshakeCallback){
			protocol.start(null, handshakeCallback);
		}
		
		public bool connect(JsonObject user, Action<JsonObject> handshakeCallback){
			try{
				protocol.start(user, handshakeCallback);
				return true;
			}catch(Exception e){
				Console.WriteLine(e.ToString());
				return false;
			}
		}

		public void request(string route, Action<JsonObject> action){
			this.request(route, new JsonObject(), action);
		}
		
		public void request(string route, JsonObject msg, Action<JsonObject> action){
			this.eventManager.AddCallBack(reqId, action);
			protocol.send (route, reqId, msg);

			reqId++;
		}
		
		public void notify(string route, JsonObject msg){
			protocol.send (route, msg);
		}
		
		public void on(string eventName, Action<JsonObject> action){
			eventManager.AddOnEvent(eventName, action);
		}

		internal void processMessage(Message msg){
#if LUZEXI
			lock(this.m_cLock)
			{
				this.m_seqReceiveMessage.Enqueue(msg);
			}
#else
			if(msg.type == MessageType.MSG_RESPONSE){
				eventManager.InvokeCallBack(msg.id, msg.data);
			}else if(msg.type == MessageType.MSG_PUSH){
				eventManager.InvokeOnEvent(msg.route, msg.data);
			}
#endif
		}

#if LUZEXI
		/// <summary>
		/// luzexi:Update the process of the message
		/// </summary>
		public void Update()
		{
			for( int i = 0 ; i<PROCESS_NUM && i < this.m_seqReceiveMessage.Count ; i++ )
			{
				Message msg;
				//if( this.m_seqReceiveMessage.TryDequeue(out msg) )
				lock(this.m_cLock)
				{
					msg = this.m_seqReceiveMessage.Dequeue();
					{
						if(msg.type == MessageType.MSG_RESPONSE)
						{
							eventManager.InvokeCallBack(msg.id, msg.data);
						}
						else if(msg.type == MessageType.MSG_PUSH)
						{
							eventManager.InvokeOnEvent(msg.route, msg.data);
						}
					}
				}
			}
		}
#endif

		public void disconnect(){
			Dispose ();
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// The bulk of the clean-up code 
		protected virtual void Dispose(bool disposing) {
			if(this.disposed) return;

			if (disposing) {
				// free managed resources
				this.protocol.close();
				this.socket.Shutdown(SocketShutdown.Both);
				this.socket.Close();
				this.disposed = true;

				//Call disconnect callback
				eventManager.InvokeOnEvent(EVENT_DISCONNECT, null);
			}
		}
	}
}

