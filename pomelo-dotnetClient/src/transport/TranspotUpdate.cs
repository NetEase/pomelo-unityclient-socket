using UnityEngine;
using System;
using System.Collections;

//	TranspotUpdate.cs
//	Author: Lu Zexi
//	2014-6-20



namespace Pomelo.DotNetClient
{
	/// <summary>
	/// Transpot updater.
	/// </summary>
	public class TranspotUpdate : MonoBehaviour
	{
		private enum STATE
		{
			NONE = 0,
			START = 1,
			RUNING = 2,
			CLOSE = 3,
		}
		private STATE m_eStat = STATE.NONE;	//the state of the transpotUpdate
		private Action m_cUpdate;	//update action
		private Action m_cOnDisconnect;	//on the disconnect

		/// <summary>
		/// init the transport udpate to start the work.
		/// </summary>
		/// <param name="update">Update.</param>
		/// <param name="ondisconnect">Ondisconnect.</param>
		public static TranspotUpdate Init( Action update , Action ondisconnect)
		{
			GameObject obj = new GameObject("Socket");
			TranspotUpdate trans = obj.AddComponent<TranspotUpdate>();
			trans.m_cUpdate = update;
			trans.m_cOnDisconnect = ondisconnect;
			trans.m_eStat = STATE.START;
			return trans;
		}

		/// <summary>
		/// close the updater
		/// </summary>
		public void Close()
		{
			this.m_eStat = STATE.CLOSE;
		}

		/// <summary>
		/// Fixeds the update.
		/// </summary>
		void FixedUpdate()
		{
			switch(this.m_eStat)
			{
			case STATE.START:
			case STATE.RUNING:
				if(this.m_cUpdate != null )
				{
					this.m_cUpdate();
				}
				break;
			case STATE.CLOSE:
				this.m_eStat = STATE.NONE;
				this.m_cOnDisconnect();
				GameObject.Destroy(gameObject);
				break;
			}
		}
	}
}

