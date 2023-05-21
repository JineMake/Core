using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetClient.GetInstance.Init();
        C_Fly.GetInstance.Init();
    }

    // Update is called once per frame
    void Update()
    {
        NetClient.GetInstance.Updata();
    }
}
