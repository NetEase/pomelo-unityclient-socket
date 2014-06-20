#define LUZEXI

using System;
using System.Collections.Generic;
using System.Net.Sockets;

#if LUZEXI
using UnityEngine;
#endif

namespace Pomelo.DotNetClient
{
	class StateObject
	{
		public const int BufferSize = 1024;
		internal byte[] buffer = new byte[BufferSize];
	}
	
	public class Transporter
	{
		public const int HeadLength = 4;
		
		private Socket socket;
		private Action<byte[]> messageProcesser;
		
		//Used for get message
		private StateObject stateObject = new StateObject();
		private TransportState transportState;
		private IAsyncResult asyncReceive;
		private IAsyncResult asyncSend;
		private bool onSending = false;
		private bool onReceiving = false;
		private byte[] headBuffer = new byte[4];
		private byte[] buffer;
		private int bufferOffset = 0;
		private int pkgLength = 0;
		internal Action onDisconnect = null;

#if LUZEXI
		private System.Object m_cLock = new System.Object();	//the lock object
		private const int PROCESS_NUM = 5;	//the process handle num per fps
		private Queue<byte[]> m_seqReceiveMsg = new Queue<byte[]>();	//the message queue
		private TranspotUpdate m_cUpdater = null;	//The Updater of the message queue
#endif
		
		public Transporter (Socket socket, Action<byte[]> processer){
			this.socket = socket;
			this.messageProcesser = processer;
			transportState = TransportState.readHead;
		}
		
		public void start(){
#if LUZEXI
			this.m_cUpdater = TranspotUpdate.Init(Update , OnDisconnect);
#endif
			this.receive();
		}
		
		public void send(byte[] buffer){
			if(this.transportState != TransportState.closed){ 
				this.asyncSend = socket.BeginSend (buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback (sendCallback), socket);
				this.onSending = true;
			}
		}
		
		private void sendCallback(IAsyncResult asyncSend){
			if(this.transportState == TransportState.closed) return;
			socket.EndSend (asyncSend);
			this.onSending = false;
		}
		
		public void receive(){
			//Console.WriteLine("receive state : {0}, {1}", this.transportState, socket.Available);
			this.asyncReceive = socket.BeginReceive(stateObject.buffer, 0, stateObject.buffer.Length, SocketFlags.None, new AsyncCallback(endReceive), stateObject);
			this.onReceiving = true;
		}
		
		internal void close(){
			this.transportState = TransportState.closed;
#if LUZEXI
			if(this.m_cUpdater != null )
			{
				this.m_cUpdater.Close();
				this.m_cUpdater = null;
			}
#endif
			/*try{
				if(this.onReceiving) socket.EndReceive (this.asyncReceive);
				if(this.onSending) socket.EndSend(this.asyncSend);
			}catch (Exception e){
				Console.WriteLine(e.Message);
			}*/
		}
		
		private void endReceive(IAsyncResult asyncReceive){
			if(this.transportState == TransportState.closed) return;
			StateObject state = (StateObject)asyncReceive.AsyncState;
			Socket socket = this.socket;
			int length = socket.EndReceive (asyncReceive);
			this.onReceiving = false;
			
			if (length > 0) {
				processBytes(state.buffer, 0, length);
				//Receive next message
				if(this.transportState != TransportState.closed) receive ();
			} else{
#if LUZEXI
				this.m_cUpdater.Close();
				this.m_cUpdater = null;
#else
				if(this.onDisconnect != null) this.onDisconnect();
#endif
			}
		}
		
		internal void processBytes(byte[] bytes, int offset, int limit){
			if (this.transportState == TransportState.readHead) {
				readHead (bytes, offset, limit);
			} else if (this.transportState == TransportState.readBody) {
				readBody (bytes, offset, limit);
			}
		}
		
		private bool readHead(byte[] bytes, int offset, int limit){
			int length = limit - offset;
			int headNum = HeadLength - bufferOffset;
			
			if(length >= headNum){
				//Write head buffer
				writeBytes(bytes, offset, headNum, bufferOffset, headBuffer);
				//Get package length
				pkgLength = (headBuffer [1] << 16) + (headBuffer [2] << 8) + headBuffer [3];
				
				//Init message buffer
				buffer = new byte[HeadLength + pkgLength];
				writeBytes (headBuffer, 0, HeadLength, buffer);
				offset += headNum;
				bufferOffset = HeadLength;
				this.transportState = TransportState.readBody;
				
				if(offset <= limit) processBytes(bytes, offset, limit);
				return true;
			}else{
				writeBytes(bytes, offset, length, bufferOffset, headBuffer);
				bufferOffset += length;
				return false;
			}
		}
		
		private void readBody(byte[] bytes, int offset, int limit){
			int length = pkgLength + HeadLength - bufferOffset;
			if ((offset + length) <= limit) {
				writeBytes (bytes, offset, length, bufferOffset, buffer);
				offset += length;

#if LUZEXI
				lock(this.m_cLock)
				{
					this.m_seqReceiveMsg.Enqueue(buffer);
				}
#else
				//Invoke the protocol api to handle the message
				this.messageProcesser.Invoke(buffer);
#endif
				this.bufferOffset = 0;
				this.pkgLength = 0;
				
				if(this.transportState != TransportState.closed) this.transportState = TransportState.readHead;
				if(offset< limit) processBytes(bytes, offset, limit); 
			} else {
				writeBytes (bytes, offset, limit - offset, bufferOffset, buffer);
				bufferOffset += limit - offset;
				this.transportState = TransportState.readBody;
			}
		}
		
		private void writeBytes(byte [] source, int start, int length, byte [] target){
			writeBytes(source, start, length, 0, target);
		}
		
		private void writeBytes(byte [] source, int start, int length, int offset, byte [] target){
			for(int i = 0; i < length; i++){
				target[offset + i] = source[start + i];
			}
		}
		
		private void print(byte[] bytes, int offset, int length){
			for(int i = offset; i < length; i++)
				Console.Write(Convert.ToString(bytes[i], 16) + " ");
			Console.WriteLine();
		}

#if LUZEXI
		internal void Update()
		{
			for( int i = 0 ; i<PROCESS_NUM && i< this.m_seqReceiveMsg.Count; i++)
			{
				lock(this.m_cLock)
				{
					byte[] data = this.m_seqReceiveMsg.Dequeue();
					this.messageProcesser.Invoke(data);
				}
			}
		}

		/// <summary>
		/// Raises the disconnect event.
		/// </summary>
		internal void OnDisconnect()
		{
			if(this.onDisconnect != null) this.onDisconnect();
		}
#endif
	}
}