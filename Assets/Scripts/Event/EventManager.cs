using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3Game.Event
{
     //关于事件id，使用int类型(有必要的话甚至可以使用short)的原因是比使用string来说性能相对较好
    //事件都存储在字典中，int做key查找要比string做key快
    //仅需要维护c#一个事件id的枚举即可，lua则维护一个事件id table即可，两侧均使用时，lua中注册的事件必须与c#一致
    
    public class EventManager
    {
        private static EventManager _instance = null;
        private static object _lock = new object();

        public static EventManager instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new EventManager();
                        }
                    }
                }

                return _instance;
            }
        }
        
        /// <summary>
        /// 事件id-参数类型集合的字典
        /// 注意:
        /// 无参的事件id对应的Type[] == null 
        /// </summary>
        private Dictionary<int,Type[]> _eventParamsTypeDic;
        
        //备注 本类中请严谨的区分Dictionary.ContainsKey()和 Dictionary[key] == ？做判断
        //以下两个字典均为事件id-event的集合，却别为
        //_eventDic保存所有的无参事件(c# and lua)以及所有带参的C#函数
        //_eventDic保存所有带参的lua函数
        private Dictionary<int,Delegate> _eventDic;
        private Dictionary<int,Delegate> _eventLuaDic;
        private  EventManager(){}

        /// <summary>
        /// 初始化函数
        /// </summary>
        public void Init()
        {
            _eventParamsTypeDic = new Dictionary<int, Type[]>();
            _eventDic = new Dictionary<int, Delegate>();
            _eventLuaDic = new Dictionary<int, Delegate>();
        }
        public void Clear()
        {
            _eventDic.Clear();
            _eventLuaDic.Clear();
            _eventParamsTypeDic.Clear();
            _eventDic = null;
            _eventLuaDic = null;
            _eventParamsTypeDic = null;
        }

        #region 注册事件
        /// <summary>
        /// 注册事件
        /// 注意:
        /// 无参的事件C#和Lua函数均保存在_eventDic中
        /// 无参的情况下callback无需区分
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="callback"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddListener(int eventId, Action callback)
        {
            if (!_eventParamsTypeDic.ContainsKey(eventId))
            {
                _eventParamsTypeDic.Add(eventId, null);
                _eventDic.Add(eventId, null);
            }

            if (_eventParamsTypeDic[eventId] == null)
            {
                _eventDic[eventId] = (Action)_eventDic[eventId] + callback;
            }
            else
            {
                Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
            }
        }

        public void AddListener<T>(int eventId, Action<T> callback)
        {
            if (!_eventParamsTypeDic.ContainsKey(eventId))
            {
                _eventParamsTypeDic.Add(eventId, new[] { typeof(T) });
                _eventDic.Add(eventId, null);
                _eventLuaDic.Add(eventId, null);
            }

            if (_eventParamsTypeDic[eventId][0] == typeof(T))
            {
                _eventDic[eventId] = (Action<T>)_eventDic[eventId] + callback;
            }
            else
            {
                Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
            }
        }
        public void AddListener(int eventId, Action<object> callback, Type type)
        {
            if (!_eventParamsTypeDic.ContainsKey(eventId))
            {
                _eventParamsTypeDic.Add(eventId, new[] { type });
                _eventDic.Add(eventId, null);
                _eventLuaDic.Add(eventId, null);
            }

            if (_eventParamsTypeDic[eventId][0] == type)
            {
                _eventLuaDic[eventId] = Delegate.Combine(_eventLuaDic[eventId], callback);
            }
            else
            {
                Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
            }
        }

        public void AddListener<T1, T2>(int eventId, Action<T1, T2> callback)
        {
            if (!_eventParamsTypeDic.ContainsKey(eventId))
            {
                _eventParamsTypeDic.Add(eventId, new[] { typeof(T1), typeof(T2) });
                _eventDic.Add(eventId, null);
                _eventLuaDic.Add(eventId, null);
            }

            if (_eventParamsTypeDic[eventId][0] == typeof(T1) && _eventParamsTypeDic[eventId][1] == typeof(T2))
            {
                _eventDic[eventId] = (Action<T1, T2>)_eventDic[eventId] + callback;
            }
            else
            {
                Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
            }
        }
        public void AddListener(int eventId, Action<object, object> callback, Type type1, Type type2)
        {
            if (!_eventParamsTypeDic.ContainsKey(eventId))
            {
                _eventParamsTypeDic.Add(eventId, new[] { type1, type2 });
                _eventDic.Add(eventId, null);
                _eventLuaDic.Add(eventId, null);
            }

            if (_eventParamsTypeDic[eventId][0] == type1 && _eventParamsTypeDic[eventId][1] == type2)
            {
                _eventLuaDic[eventId] = Delegate.Combine(_eventLuaDic[eventId], callback);
            }
            else
            {
                Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
            }
        }
        public void AddListener(int eventId, Action<object, object> callback, Type[] types)
        {
            if (!_eventParamsTypeDic.ContainsKey(eventId))
            {
                _eventParamsTypeDic.Add(eventId, types);
                _eventDic.Add(eventId, null);
                _eventLuaDic.Add(eventId, null);
            }

            if (_eventParamsTypeDic[eventId][0] == types[0] && _eventParamsTypeDic[eventId][1] == types[1])
            {
                _eventLuaDic[eventId] = Delegate.Combine(_eventLuaDic[eventId], callback);
            }
            else
            {
                Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
            }
        }

        public void AddListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> callback)
        {
            if (!_eventParamsTypeDic.ContainsKey(eventId))
            {
                _eventParamsTypeDic.Add(eventId, new[] { typeof(T1), typeof(T2), typeof(T3) });
                _eventDic.Add(eventId, null);
                _eventLuaDic.Add(eventId, null);
            }

            if (_eventParamsTypeDic[eventId][0] == typeof(T1) && _eventParamsTypeDic[eventId][1] == typeof(T2) && _eventParamsTypeDic[eventId][2] == typeof(T3))
            {
                _eventDic[eventId] = (Action<T1, T2, T3>)_eventDic[eventId] + callback;
            }
            else
            {
                Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
            }
        }
        public void AddListener(int eventId, Action<object, object, object> callback, Type type1, Type type2, Type type3)
        {
            if (!_eventParamsTypeDic.ContainsKey(eventId))
            {
                _eventParamsTypeDic.Add(eventId, new[] { type1, type2, type3 });
                _eventDic.Add(eventId, null);
                _eventLuaDic.Add(eventId, null);
            }

            if (_eventParamsTypeDic[eventId][0] == type1 && _eventParamsTypeDic[eventId][1] == type2 && _eventParamsTypeDic[eventId][2] == type3)
            {
                _eventLuaDic[eventId] = Delegate.Combine(_eventLuaDic[eventId], callback);
            }
            else
            {
                Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
            }
        }
        public void AddListener(int eventId, Action<object, object, object> callback, Type[] types)
        {
            if (!_eventParamsTypeDic.ContainsKey(eventId))
            {
                _eventParamsTypeDic.Add(eventId, types);
                _eventDic.Add(eventId, null);
                _eventLuaDic.Add(eventId, null);
            }

            if (_eventParamsTypeDic[eventId][0] == types[0] && _eventParamsTypeDic[eventId][1] == types[1] && _eventParamsTypeDic[eventId][2] == types[2])
            {
                _eventLuaDic[eventId] = Delegate.Combine(_eventLuaDic[eventId], callback);
            }
            else
            {
                Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
            }
        }
        #endregion

        #region 移除事件

        public void RemoveListener(int eventId, Action callback)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId] == null)
                {
                    _eventDic[eventId] = Delegate.Remove(_eventDic[eventId], callback);
                    if (_eventDic[eventId] == null)
                    {
                        _eventDic.Remove(eventId);
                        _eventParamsTypeDic.Remove(eventId);
                    }
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件Id不存在 eventId: {eventId}");
            }
        }

        public void RemoveListener<T>(int eventId, Action<T> callback)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == typeof(T))
                {
                    _eventDic[eventId] = Delegate.Remove(_eventDic[eventId], callback);
                    if (_eventDic[eventId] == null && _eventLuaDic[eventId] == null)
                    {
                        _eventDic.Remove(eventId);
                        _eventParamsTypeDic.Remove(eventId);
                    }
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件Id不存在 eventId: {eventId}");
            }
        }
        public void RemoveListener(int eventId, Action<object> callback, Type type)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == type)
                {
                    _eventLuaDic[eventId] = Delegate.Remove(_eventLuaDic[eventId], callback);
                    if (_eventDic[eventId] == null && _eventLuaDic[eventId] == null)
                    {
                        _eventDic.Remove(eventId);
                        _eventParamsTypeDic.Remove(eventId);
                    }
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件Id不存在 eventId: {eventId}");
            }
        }

        public void RemoveListener<T1, T2>(int eventId, Action<T1, T2> callback)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == typeof(T1) && _eventParamsTypeDic[eventId][1] == typeof(T2))
                {
                    _eventDic[eventId] = Delegate.Remove(_eventDic[eventId], callback);
                    if (_eventDic[eventId] == null && _eventLuaDic[eventId] == null)
                    {
                        _eventDic.Remove(eventId);
                        _eventParamsTypeDic.Remove(eventId);
                    }
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件Id不存在 eventId: {eventId}");
            }
        }
        public void RemoveListener(int eventId, Action<object, object> callback, Type type1, Type type2)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == type1 && _eventParamsTypeDic[eventId][1] == type2)
                {
                    _eventLuaDic[eventId] = Delegate.Remove(_eventLuaDic[eventId], callback);
                    if (_eventDic[eventId] == null && _eventLuaDic[eventId] == null)
                    {
                        _eventDic.Remove(eventId);
                        _eventParamsTypeDic.Remove(eventId);
                    }
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件Id不存在 eventId: {eventId}");
            }
        }
        public void RemoveListener(int eventId, Action<object, object> callback, Type[] types)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == types[0] && _eventParamsTypeDic[eventId][1] == types[1])
                {
                    _eventLuaDic[eventId] = Delegate.Remove(_eventLuaDic[eventId], callback);
                    if (_eventDic[eventId] == null && _eventLuaDic[eventId] == null)
                    {
                        _eventDic.Remove(eventId);
                        _eventParamsTypeDic.Remove(eventId);
                    }
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件Id不存在 eventId: {eventId}");
            }
        }

        public void RemoveListener<T1, T2, T3>(int eventId, Action<T1, T2, T3> callback)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == typeof(T1) && _eventParamsTypeDic[eventId][1] == typeof(T2) && _eventParamsTypeDic[eventId][2] == typeof(T3))
                {
                    _eventDic[eventId] = Delegate.Remove(_eventDic[eventId], callback);
                    if (_eventDic[eventId] == null && _eventLuaDic[eventId] == null)
                    {
                        _eventDic.Remove(eventId);
                        _eventParamsTypeDic.Remove(eventId);
                    }
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件Id不存在 eventId: {eventId}");
            }
        }
        public void RemoveListener(int eventId, Action<object, object, object> callback, Type type1, Type type2, Type type3)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == type1 && _eventParamsTypeDic[eventId][1] == type2 && _eventParamsTypeDic[eventId][2] == type3)
                {
                    _eventLuaDic[eventId] = Delegate.Remove(_eventLuaDic[eventId], callback);
                    if (_eventDic[eventId] == null && _eventLuaDic[eventId] == null)
                    {
                        _eventDic.Remove(eventId);
                        _eventParamsTypeDic.Remove(eventId);
                    }
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件Id不存在 eventId: {eventId}");
            }
        }
        public void RemoveListener(int eventId, Action<object, object, object> callback, Type[] types)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == types[0] && _eventParamsTypeDic[eventId][1] == types[1] && _eventParamsTypeDic[eventId][2] == types[2])
                {
                    _eventLuaDic[eventId] = Delegate.Remove(_eventLuaDic[eventId], callback);
                    if (_eventDic[eventId] == null && _eventLuaDic[eventId] == null)
                    {
                        _eventDic.Remove(eventId);
                        _eventParamsTypeDic.Remove(eventId);
                    }
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件Id不存在 eventId: {eventId}");
            }
        }

        /// <summary>
        /// 移除指定id的事件
        /// 不要轻易使用该方法，除非确定所有方法需要移除
        /// </summary>
        /// <param name="eventId"></param>
        public void RemoveAllListener(int eventId)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventDic.ContainsKey(eventId))
                {
                    if (_eventDic[eventId] != null)
                    {
                        _eventDic[eventId] = Delegate.RemoveAll(_eventDic[eventId], _eventDic[eventId]);
                    }
                    _eventDic.Remove(eventId);
                }
                if (_eventLuaDic.ContainsKey(eventId))
                {
                    if(_eventLuaDic[eventId] != null)
                    {
                        _eventLuaDic[eventId] = Delegate.RemoveAll(_eventLuaDic[eventId],_eventLuaDic[eventId]);
                    }
                    _eventLuaDic.Remove(eventId);
                }
                _eventParamsTypeDic.Remove(eventId);
            }
        }
        #endregion

        #region 执行事件

        public void TriggerEvent(int eventId)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId] == null)
                {
                    ((Action)_eventDic[eventId]).Invoke();
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}, eventId: {(EEventId)eventId}");
                }
            }
            else
            {
                Debug.LogWarning($"事件id不存在，请查询是否注册事件！！！ eventId: {(EEventId)eventId}");
            }
        }

        public void TriggerEvent<T>(int eventId, T t)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == typeof(T))
                {
                    ((Action<T>)_eventDic[eventId]).Invoke(t);
                    _eventLuaDic[eventId]?.DynamicInvoke(t);
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件id不存在，请查询是否注册事件！！！ eventId: {eventId}");
            }
        }
        public void TriggerEvent(int eventId, object o, Type type)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == type)
                {
                    _eventLuaDic[eventId].DynamicInvoke(o);
                    _eventDic[eventId]?.DynamicInvoke(o);
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件id不存在，请查询是否注册事件！！！ eventId: {eventId}");
            }
        }

        public void TriggerEvent<T1, T2>(int eventId, T1 t1, T2 t2)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == typeof(T1) && _eventParamsTypeDic[eventId][1] == typeof(T2))
                {
                    ((Action<T1, T2>)_eventDic[eventId]).Invoke(t1, t2);
                    _eventLuaDic[eventId]?.DynamicInvoke(t1, t2);
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件id不存在，请查询是否注册事件！！！ eventId: {eventId}");
            }
        }
        public void TriggerEvent(int eventId, object o1, object o2, Type type1, Type type2)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == type1 && _eventParamsTypeDic[eventId][1] == type2)
                {
                    _eventLuaDic[eventId].DynamicInvoke(o1, o2);
                    _eventDic[eventId]?.DynamicInvoke(o1, o2);
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件id不存在，请查询是否注册事件！！！ eventId: {eventId}");
            }
        }
        public void TriggerEvent(int eventId, object o1, object o2, Type[] types)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == types[0] && _eventParamsTypeDic[eventId][1] == types[1])
                {
                    _eventLuaDic[eventId].DynamicInvoke(o1, o2);
                    _eventDic[eventId]?.DynamicInvoke(o1, o2);
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件id不存在，请查询是否注册事件！！！ eventId: {eventId}");
            }
        }

        public void TriggerEvent<T1, T2, T3>(int eventId, T1 t1, T2 t2, T3 t3)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == typeof(T1) && _eventParamsTypeDic[eventId][1] == typeof(T2) && _eventParamsTypeDic[eventId][2] == typeof(T3))
                {
                    ((Action<T1, T2, T3>)_eventDic[eventId]).Invoke(t1, t2, t3);
                    _eventLuaDic[eventId]?.DynamicInvoke(t1, t2);
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件id不存在，请查询是否注册事件！！！ eventId: {eventId}");
            }
        }
        public void TriggerEvent(int eventId, object o1, object o2, object o3, Type type1, Type type2, Type type3)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == type1 && _eventParamsTypeDic[eventId][1] == type2 && _eventParamsTypeDic[eventId][2] == type3)
                {
                    _eventLuaDic[eventId].DynamicInvoke(o1, o2, o3);
                    _eventDic[eventId]?.DynamicInvoke(o1, o2, o3);
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件id不存在，请查询是否注册事件！！！ eventId: {eventId}");
            }
        }
        public void TriggerEvent(int eventId, object o1, object o2, object o3, Type[] types)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (_eventParamsTypeDic[eventId][0] == types[0] && _eventParamsTypeDic[eventId][1] == types[1] && _eventParamsTypeDic[eventId][2] == types[2])
                {
                    _eventLuaDic[eventId]?.DynamicInvoke(o1, o2, o3);
                    _eventDic[eventId]?.DynamicInvoke(o1, o2, o3);
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件id不存在，请查询是否注册事件！！！ eventId: {eventId}");
            }
        }
        
        public void TriggerEvent(int eventId,object[] values,Type[] types)
        {
            if (_eventParamsTypeDic.ContainsKey(eventId))
            {
                if (CheckEventType(eventId, types))
                {
                    _eventDic[eventId]?.DynamicInvoke(values);
                    _eventLuaDic[eventId]?.DynamicInvoke(values);
                }
                else
                {
                    Debug.LogWarning($"事件id对应的事件类型不一致，callback type: {_eventDic[eventId].GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"事件id不存在，请查询是否注册事件！！！ eventId: {eventId}");
            }
        }
        #endregion

        private bool CheckEventType(int eventId,Type[] types)
        {
            Type[] sourceTypes = _eventParamsTypeDic[eventId];
            if(sourceTypes.Length != types.Length)
            {
                return false;
            }

            for (int i = 0; i < sourceTypes.Length; i++)
            {
                if(sourceTypes[i] != types[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}