using System;
using System.Timers;

namespace Pomelo.DotNetClient
{
	public class HeartBeatService
	{
		int interval;
		public int timeout;
		Timer timer;

		Protocol protocol;
		
		public HeartBeatService(int timeout, Protocol protocol){
			this.interval = timeout*1000;
			this.protocol = protocol;
		}

		internal void resetTimeout(){
			this.timeout = 0;
		}

		public void sendHeartBeat(object source, ElapsedEventArgs e){
			//check timeout
			if(timeout > interval*2){
				protocol.close();	
			}

			//Send heart beat
			protocol.send(PackageType.PKG_HEARTBEAT);
		}

		public void start(){
			if(interval < 1000) return;

			//start hearbeat
			this.timer = new Timer();
			timer.Interval = interval;
			timer.Elapsed += new ElapsedEventHandler(sendHeartBeat);
			timer.Enabled = true;

			//Set timeout
			timeout = 0;
		}

		public void stop(){
			if(this.timer != null) {
				this.timer.Enabled = false;
				this.timer.Dispose();
			}
		}
	}
}

