using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Fly : Singleton<C_Fly>
{

    GameObject plane;
    public void Init()
    {
        plane = GameObject.FindGameObjectWithTag("Player");
        MessageCenter.GetInstance.AddListener(MsgID.S2C_MovePostion, MovePos);
    }
    private void MovePos(object obj)
    {
        Debug.Log("开始同步移动");
        object[] arr = obj as object[];
        byte[] data = arr[0] as byte[];
        PlaneFly fly = PlaneFly.Parser.ParseFrom(data);
        plane.transform.position = new Vector3(fly.Px, fly.Py, fly.Pz);
        plane.transform.eulerAngles = new Vector3(fly.Rx, fly.Ry, fly.Rz);
    }
}
