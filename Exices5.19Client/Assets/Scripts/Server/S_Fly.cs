using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;

public class S_Fly : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * 10);
        transform.Rotate(-transform.forward * Input.GetAxis("Horizontal") * Time.deltaTime * 50);
        transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * Time.deltaTime * 50);
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            OnSendPosAllRos(transform.position, transform.eulerAngles);
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
        NetClient.GetInstance.Send(MsgID.S2C_MovePostion, fly.ToByteArray());
    }
}
