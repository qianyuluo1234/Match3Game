using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Match3Game.UnityEx
{
    public static class EventTriggerCenter
    {
        public static void AddListener(this EventTrigger trigger, UnityAction<BaseEventData> action, EventTriggerType triggerType)
        {
            if (trigger == null)
            {
                return;
            }

            if (!FindEventTriggerEntry(trigger, triggerType, out EventTrigger.Entry entry))
            {
                entry = new EventTrigger.Entry();
                entry.eventID = triggerType;
                trigger.triggers.Add(entry);
            }
            
            entry.callback.AddListener(action);
        }

        public static void RemoveListener(this EventTrigger trigger, UnityAction<BaseEventData> action, EventTriggerType triggerType)
        {
            if (trigger == null || !FindEventTriggerEntry(trigger, triggerType, out EventTrigger.Entry entry))
            {
                return;
            }
            entry.callback.RemoveListener(action);
        }
        private static bool FindEventTriggerEntry(EventTrigger trigger, EventTriggerType triggerType,out EventTrigger.Entry entry)
        {
            entry = trigger.triggers.Find(_ => _.eventID == triggerType);
            
            return entry != null;
        }
    }
}