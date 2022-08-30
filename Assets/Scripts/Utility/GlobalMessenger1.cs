﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public static class GlobalMessenger
{
    private class EventData
    {
        public List<Callback> callbacks = new List<Callback>();
        public List<Callback> temp = new List<Callback>();
        public bool isInvoking;
    }

    private static Dictionary<EventMsg, EventData> _eventDic = new Dictionary<EventMsg, EventData>();

    public static void AddListener(EventMsg msg, Callback handler) {
        Dictionary<EventMsg, EventData> obj = _eventDic;
        lock (obj)
        {
            EventData eventData;
            if (!_eventDic.TryGetValue(msg, out eventData))
            {
                eventData = new EventData();
                _eventDic.Add(msg, eventData);
            }
            eventData.callbacks.Add(handler);
        }
    }

    public static void RemoveListener(EventMsg msg, Callback handler) {
        Dictionary<EventMsg, EventData> obj = _eventDic;
        lock (obj)
        {
            EventData eventData;
            if (_eventDic.TryGetValue(msg, out eventData))
            {
                int num = eventData.callbacks.IndexOf(handler);
                if (num >= 0)
                {
                    eventData.callbacks[num] = eventData.callbacks[eventData.callbacks.Count - 1];
                    eventData.callbacks.RemoveAt(eventData.callbacks.Count - 1);
                }
            }
        }
    }

    public static void Launch(EventMsg msg) {
        Dictionary<EventMsg, EventData> obj = _eventDic;
        lock (obj)
        {
            EventData eventData;
            if (_eventDic.TryGetValue(msg, out eventData))
            {
                if (eventData.isInvoking)
                {
                    throw new InvalidOperationException("Can not support Launch calls to the same eventType.");
                }
                eventData.isInvoking = true;
                eventData.temp.AddRange(eventData.callbacks);
                for (int i = 0; i < eventData.temp.Count; i++)
                {
                    try
                    {
                        eventData.temp[i]();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
                eventData.temp.Clear();
                eventData.isInvoking = false;
            }
        }
    }

    public static void ClearALL() {
        _eventDic.Clear();
    }

}
