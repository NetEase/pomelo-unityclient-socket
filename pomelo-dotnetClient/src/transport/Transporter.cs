using System;
using System.Net.Sockets;

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

		public Transporter (Socket socket, Action<byte[]> processer){
			this.socket = socket;
			this.messageProcesser = processer;
			transportState = TransportState.readHead;
		}

		public void start(){
			this.receive ();
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
			if(this.onReceiving) socket.EndReceive (this.asyncReceive);
			if(this.onSending) socket.EndSend(this.asyncSend);
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
				Console.WriteLine("server disconnect !");
			}
		}

		internal void processBytes(byte[] bytes, int pos, int length){
			if (this.transportState == TransportState.readHead) {
				readHead (bytes, pos, length);
			} else if (this.transportState == TransportState.readBody) {
				readBody (bytes, pos, length);
			}
		}

		private bool readHead(byte[] bytes, int pos, int length){
			int byteNum = length - pos;
			int headNum = HeadLength - bufferOffset;

			if(byteNum >= headNum){
				//Write head buffer
				writeBytes(bytes, pos, headNum, headBuffer);
				//Get package length
				pkgLength = (headBuffer [1] << 16) + (headBuffer [2] << 8) + headBuffer [3];

				//Init message buffer
				buffer = new byte[HeadLength + pkgLength];
				writeBytes (headBuffer, 0, headNum, buffer);
				pos += headNum;
				bufferOffset = HeadLength;
				this.transportState = TransportState.readBody;

				if(pos < length) processBytes(bytes, pos, length);
				return true;
			}else{
				writeBytes(bytes, pos, byteNum, headBuffer);
				bufferOffset += byteNum;
				return false;
			}
		}

		private void readBody(byte[] bytes, int pos, int length){
			if ((pos + pkgLength) <= length) {
				writeBytes (bytes, pos, pkgLength, HeadLength, buffer);
				pos = pos + pkgLength;

				//Invoke the protocol api to handle the message
				this.messageProcesser.Invoke(buffer);
				this.bufferOffset = 0;
				this.pkgLength = 0;

				if(this.transportState != TransportState.closed) this.transportState = TransportState.readHead;
				if(pos< length) processBytes(bytes, pos, length); 
			} else {
				writeBytes (bytes, pos, length - pos, buffer);
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
	}
}

