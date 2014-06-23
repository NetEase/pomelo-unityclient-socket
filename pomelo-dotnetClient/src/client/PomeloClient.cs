#define LUZEXI

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
		TranspotUpdate m_cUpdater;	//the updater of the process message
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
#if LUZEXI
			connect(null , null);
#else
			protocol.start(null, null);
#endif
		}
		
		public void connect(JsonObject user){
#if LUZEXI
			connect(user , null);
#else
			protocol.start(user, null);
#endif
		}
		
		public void connect(Action<JsonObject> handshakeCallback){
#if LUZEXI
			connect(null ,handshakeCallback);
#else
			protocol.start(null, handshakeCallback);
#endif
		}
		
		public bool connect(JsonObject user, Action<JsonObject> handshakeCallback){
			try{
#if LUZEXI
				this.m_cUpdater = TranspotUpdate.Init();
				protocol.start(user, handshakeCallback , this.m_cUpdater);
#else
				protocol.start(user, handshakeCallback);
#endif
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
			if(msg.type == MessageType.MSG_RESPONSE){
				eventManager.InvokeCallBack(msg.id, msg.data);
			}else if(msg.type == MessageType.MSG_PUSH){
				eventManager.InvokeOnEvent(msg.route, msg.data);
			}
		}

#if LUZEXI
		/// <summary>
		/// Determines whether this socket is connect.
		/// </summary>
		/// <returns><c>true</c> if this instance is connect; otherwise, <c>false</c>.</returns>
		public bool IsConnect()
		{
			if( this.protocol != null && this.protocol.GetState() == ProtocolState.working )
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Raises the disconnect event.
		/// </summary>
		internal void OnDisconnect()
		{
			List<Action<JsonObject>> lst = eventManager.GetEvent(EVENT_DISCONNECT);
			this.m_cUpdater.SetOndisconnect(lst);
			this.m_cUpdater.Close();
			disconnect();
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
#if LUZEXI
				this.m_cUpdater._Destory();
				this.m_cUpdater =  null;

				// free managed resources
				this.protocol.close();
				this.socket.Shutdown(SocketShutdown.Both);
				this.socket.Close();
				this.disposed = true;
#else
				// free managed resources
				this.protocol.close();
				this.socket.Shutdown(SocketShutdown.Both);
				this.socket.Close();
				this.disposed = true;

				//Call disconnect callback
				eventManager.InvokeOnEvent(EVENT_DISCONNECT, null);
#endif
			}
		}
	}
}

