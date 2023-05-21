using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;

public class NetSever : Singleton<NetSever>
{
    public Socket m_Socket;
    public List<Client> m_List = new List<Client>();
    public void Init()
    {
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 3000);
        m_Socket.Bind(iPEndPoint);
        m_Socket.Listen(100);
        m_Socket.BeginAccept(OnAccept, null);
        Debug.Log("服务器已启动...");
    }

    private void OnAccept(IAsyncResult ar)
    {
        Client client = new Client();
        client.m_Socket = m_Socket.EndAccept(ar);//链接
        IPEndPoint iPEndPoint = client.m_Socket.RemoteEndPoint as IPEndPoint;
        client.m_StrIp = iPEndPoint.Address + "" + iPEndPoint.Port;
        Debug.Log(client.m_StrIp + "已链接...");
        MessageCenter.GetInstance.BroadCast(MsgID.C2S_Accept);
        client.m_Socket.BeginReceive(client.m_Data, 0, client.m_Data.Length, SocketFlags.None, OnReceive, client);
        m_List.Add(client);
        m_Socket.BeginAccept(OnAccept, null);

    }

    private void OnReceive(IAsyncResult ar)
    {
        Client client = ar.AsyncState as Client;
        int len = client.m_Socket.EndReceive(ar);
        if (len > 0)
        {
            byte[] data = new byte[len];
            Buffer.BlockCopy(client.m_Data, 0, data, 0, len);
            client.m_Stream = client.m_Stream.Concat(data).ToArray();
            while (client.m_Stream.Length > 2)
            {
                ushort let = BitConverter.ToUInt16(client.m_Stream, 0);
                int allLen = let + 2;
                if (client.m_Stream.Length >= allLen)
                {
                    byte[] onData = new byte[let];
                    Buffer.BlockCopy(client.m_Stream, 2, onData, 0, let);
                    int id = BitConverter.ToInt32(onData, 0);
                    byte[] body = new byte[onData.Length - 4];
                    Buffer.BlockCopy(onData, 4, body, 0, body.Length);
                    MessageCenter.GetInstance.BroadCast(id, body, client);
                    int syLen = client.m_Stream.Length - allLen;
                    if (syLen > 0)
                    {
                        byte[] syData = new byte[syLen];
                        Buffer.BlockCopy(client.m_Stream, allLen, syData, 0, syLen);
                        client.m_Stream = syData;
                    }
                    else
                    {
                        client.m_Stream = new byte[0];
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            client.m_Socket.BeginReceive(client.m_Data, 0, client.m_Data.Length, SocketFlags.None, OnReceive, client);
        }
        else
        {
            Debug.Log(client.m_StrIp + "已离线...");
            m_List.Remove(client);
        }
    }

    public void AllSend(int id, byte[] body)
    {
        Debug.Log("AllSend");
        foreach (var item in m_List)
        {
            Send(id, body, item);
        }
    }
    public void Send(int id, byte[] body, Client client)
    {
        Debug.Log("Send");
        byte[] head = BitConverter.GetBytes(id);
        byte[] len = BitConverter.GetBytes((ushort)(head.Length + body.Length));
        byte[] data = new byte[0];
        data = data.Concat(len).ToArray();
        data = data.Concat(head).ToArray();
        data = data.Concat(body).ToArray();
        client.m_Socket.BeginSend(data, 0, data.Length, SocketFlags.None, OnSend, client);
    }

    private void OnSend(IAsyncResult ar)
    {
        Debug.Log("OnSend");
        Client client = ar.AsyncState as Client;
        int len = client.m_Socket.EndSend(ar);
        Debug.Log("服务器发送的长度:" + len);
    }

}
