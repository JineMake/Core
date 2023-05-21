using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MessageCenter : Singleton<MessageCenter>
{
    public Dictionary<int, Action<object>> dic = new Dictionary<int, Action<object>>();
    /// <summary>
    /// 添加事件侦听
    /// </summary>
    public void AddListener(int id,Action<object> act)
    {
        if (dic.ContainsKey(id))
        {
            dic[id] += act;
        }
        else
        {
            dic.Add(id,act);
        }
    }
    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="act"></param>
    public void RemoveListener(int id, Action<object> act)
    {
        if (dic.ContainsKey(id))
        {
            dic[id] -= act;
            if (dic[id] == null) 
            {
                dic.Remove(id);
            }
        }
    }
    /// <summary>
    /// 广播
    /// </summary>
    public void BroadCast(int id,params object[] arr)
    {
        if (dic.ContainsKey(id))
        {
            dic[id](arr);
        }
    }
}
