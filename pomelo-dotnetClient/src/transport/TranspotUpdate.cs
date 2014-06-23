using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJson;

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
		private List<Action<JsonObject>> m_cOnDisconnect;	//on the disconnect

		/// <summary>
		/// Init this instance.
		/// </summary>
		internal static TranspotUpdate Init()
		{
			GameObject obj = new GameObject("Socket");
			TranspotUpdate trans = obj.AddComponent<TranspotUpdate>();
			return trans;
		}

		/// <summary>
		/// set the update of the process message action
		/// </summary>
		/// <param name="update">Update.</param>
		internal void SetUpdate( Action update )
		{
			this.m_cUpdate = update;
		}

		/// <summary>
		/// set the disconnect evet.
		/// </summary>
		/// <param name="ondisconnect">Ondisconnect.</param>
		internal void SetOndisconnect( List<Action<JsonObject>> ondisconnect )
		{
			this.m_cOnDisconnect = ondisconnect;
		}

		/// <summary>
		/// close the updater
		/// </summary>
		internal void Close()
		{
			this.m_eStat = STATE.CLOSE;
		}

		/// <summary>
		/// Start this update.
		/// </summary>
		internal void _Start()
		{
			this.m_eStat = STATE.START;
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
				if(this.m_cOnDisconnect != null )
				{
					foreach(Action<JsonObject> action in this.m_cOnDisconnect)
						action.Invoke(null);
				}
				this.m_cOnDisconnect = null;
				GameObject.Destroy(gameObject);
				break;
			}
		}
	}
}

