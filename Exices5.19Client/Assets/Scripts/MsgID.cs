using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 定义消息协议id
/// </summary>
public class MsgID
{
    //当飞机位置发生改变时
    public const int S2C_MovePostion = 1002;
    public const int C2S_Accept = 1003;
    //当飞机转向发生改变时
    public const int S2C_MoveRotation = 1004;
}
