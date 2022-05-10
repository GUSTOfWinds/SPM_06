using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace Event
{
    public class EventSystem : MonoBehaviour
    {
    class GameListener
    {
        public EventListener listener;
        public System.Guid guid;

        public GameListener(EventListener listener)
        {
            this.listener = listener;
            this.guid = Guid.NewGuid();
        }
    }
    

        void OnEnable()
        {
            _current = this;
        }

        public static EventSystem Current
        {
            get
            {
                if (_current == null)
                {
                    _current = GameObject.FindObjectOfType<EventSystem>();
                }

                return _current;
            }
        }

        private static EventSystem _current;

        delegate void EventListener(EventInfo eventInfo);

        private Dictionary<System.Type, List<GameListener>> eventListeners;

        public void RegisterListener<T>(System.Action<T> listener, ref Guid guid) where T : EventInfo
        {
            System.Type eventType = typeof(T);
            if (eventListeners == null)
            {
                eventListeners = new Dictionary<System.Type, List<GameListener>>();
            }

            if (eventListeners.ContainsKey(eventType) == false || eventListeners[eventType] == null)
            {
                eventListeners[eventType] = new List<GameListener>();
            }

            EventListener wrapper = (eventInfo) => { listener((T) eventInfo); };
            GameListener gameListener = new GameListener(wrapper);
            eventListeners[eventType].Add(gameListener);
            guid = gameListener.guid;
        }

        public void UnregisterListener(Guid guid)
        {

            if (eventListeners == null)
            {
                return;
            }

            KeyValuePair<Type, List<GameListener>> listener = eventListeners.Where(x => x.Value.Any(y => y.guid == guid)).FirstOrDefault();

            GameListener gameListener = listener.Value.Where(x => x != null && x.guid == guid).FirstOrDefault();
            if (gameListener == null)  //no gamelistener with that Guid, just leave..
                return;
            
            eventListeners[listener.Key].Remove(gameListener);
        }

        public void FireEvent(EventInfo eventInfo)
        {
            System.Type trueEventInfoClass = eventInfo.GetType();
            if (eventListeners == null || eventListeners[trueEventInfoClass] == null)
            {
                // No one is listening, we are done.
                return;
            }
            foreach (GameListener el in eventListeners[trueEventInfoClass])
            {
                
                el.listener(eventInfo);
            }
        }
    }
}