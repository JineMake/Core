using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf;

public class S_Fly : MonoBehaviour
{
    public List<GameObject> Targets = new List<GameObject>();
    public Button otn1;
    public Button otn2;
    float time = 0;
    bool isFly = false;
    int index = 0;
    bool IsFinish = false;
    // Start is called before the first frame update
    void Start()
    {
        
        otn1.onClick.AddListener(AutoFly);
        otn2.onClick.AddListener(CommonFly);
        MessageCenter.GetInstance.AddListener(MsgID.C2S_Accept, FinishAccept);
    }

    private void FinishAccept(object obj)
    {
        Debug.Log("有客户端成功链接了");
        IsFinish = true;
    }

    /// <summary>
    /// 普通飞行
    /// </summary>
    private void CommonFly()
    {
        isFly = true;
    }
    /// <summary>
    /// 自动飞行模式
    /// </summary>
    public void AutoFly()
    {
        isFly = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (IsFinish)
        {
            if (isFly)
            {
                transform.Translate(transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * 10);
                transform.Rotate(-transform.forward * Input.GetAxis("Horizontal") * Time.deltaTime * 50);
                transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * Time.deltaTime * 50);
                if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
                {
                    OnSendPosAllRos(transform.position, transform.eulerAngles);
                }
            }
            else
            {
                time += Time.deltaTime;
                if (time <= 5)
                {

                    transform.position = Vector3.Lerp(transform.position, Targets[index].transform.position, time / 5);
                    if (Vector3.Distance(transform.position, Targets[index].transform.position) <= 1)
                    {
                        index++;
                        if (index > Targets.Count - 1)
                        {
                            index = 0;
                        }
                        time = 0;
                    }
                    OnSendPosAllRos(transform.position, transform.eulerAngles);

                }
            }
        }
       
    }


    private void OnSendPosAllRos(Vector3 pos, Vector3 ros)
    {
        PlaneFly fly = new PlaneFly();
        fly.Id = 0;
        fly.Px = pos.x;
        fly.Py = pos.y;
        fly.Pz = pos.z;
        fly.Rx = ros.x;
        fly.Ry = ros.y;
        fly.Rz = ros.z;
        NetSever.GetInstance.AllSend(MsgID.S2C_MovePostion, fly.ToByteArray());
    }
}
