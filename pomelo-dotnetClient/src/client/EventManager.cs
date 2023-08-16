using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pomelo.DotNetClient
{
    public class EventManager : IDisposable
    {
        private Dictionary<uint, Action<JObject>> callBackMap;
        private Dictionary<string, List<Action<JObject>>> eventMap;

        public EventManager()
        {
            this.callBackMap = new Dictionary<uint, Action<JObject>>();
            this.eventMap = new Dictionary<string, List<Action<JObject>>>();
        }

        //Adds callback to callBackMap by id.
        public void AddCallBack(uint id, Action<JObject> callback)
        {
            if (id > 0 && callback != null)
            {
                this.callBackMap.Add(id, callback);
            }
        }

        /// <summary>
        /// Invoke the callback when the server return messge .
        /// </summary>
        /// <param name='pomeloMessage'>
        /// Pomelo message.
        /// </param>
        public void InvokeCallBack(uint id, JObject data)
        {
            if (!callBackMap.ContainsKey(id)) return;
            callBackMap[id].Invoke(data);
        }

        //Adds the event to eventMap by name.
        public void AddOnEvent(string eventName, Action<JObject> callback)
        {
            List<Action<JObject>> list = null;
            if (this.eventMap.TryGetValue(eventName, out list))
            {
                list.Add(callback);
            }
            else
            {
                list = new List<Action<JObject>>();
                list.Add(callback);
                this.eventMap.Add(eventName, list);
            }
        }

        /// <summary>
        /// If the event exists,invoke the event when server return messge.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ///
        public void InvokeOnEvent(string route, JObject msg)
        {
            if (!this.eventMap.ContainsKey(route)) return;

            List<Action<JObject>> list = eventMap[route];
            foreach (Action<JObject> action in list) action.Invoke(msg);
        }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected void Dispose(bool disposing)
        {
            this.callBackMap.Clear();
            this.eventMap.Clear();
        }
    }
}