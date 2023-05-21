using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
public class Client : MonoBehaviour
{
    public Socket m_Socket;
    public string m_StrIp;
    public byte[] m_Data = new byte[1024];
    public byte[] m_Stream = new byte[0];
}
