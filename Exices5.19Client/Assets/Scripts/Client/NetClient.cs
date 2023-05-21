using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class NetClient : Singleton<NetClient>
{
    public Socket m_Sockets;//�ͻ��˵�ͨѶ��
    public Queue<byte[]> m_que = new Queue<byte[]>();//�ͻ��˶������ڱ���������ID�Ͱ�
    public byte[] m_Data = new byte[1024];//����
    public byte[] m_Stream = new byte[0];//��
    public void Init()
    {
        m_Sockets = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//����ͨѶ��
        m_Sockets.BeginConnect("127.0.0.1", 3000, OnConnect, null);//������������Ϣ127.0.0.1�Ǳ���ID 3000�Ƿ������˿�
    }

    private void OnConnect(IAsyncResult ar)
    {
        Debug.Log("�ɹ����ӿ�ʼ������Ϣ");
        m_Sockets.EndConnect(ar);//
        m_Sockets.BeginReceive(m_Data, 0, m_Data.Length, SocketFlags.None, OnReceive, null);//���շ��������͵�����
    }

    private void OnReceive(IAsyncResult ar)
    {
        int len = m_Sockets.EndReceive(ar);//���յĳ���
        if (len > 0)
        {
            byte[] data = new byte[len];
            Buffer.BlockCopy(m_Data, 0, data, 0, len);
            m_Stream = m_Stream.Concat(data).ToArray();
            while (m_Stream.Length > 2)
            {
                ushort bodyLen = BitConverter.ToUInt16(m_Stream, 0);
                int allLen = bodyLen + 2;
                if (m_Stream.Length >= allLen)
                {
                    byte[] oneData = new byte[bodyLen];
                    Buffer.BlockCopy(m_Stream, 2, oneData, 0, bodyLen);
                    m_que.Enqueue(oneData);
                    int syLen = m_Stream.Length - allLen;
                    if (syLen > 0)
                    {
                        byte[] syBody = new byte[syLen];
                        Buffer.BlockCopy(m_Stream, allLen, syBody, 0, syLen);
                        m_Stream = syBody;
                    }
                    else
                    {
                        m_Stream = new byte[0];
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            m_Sockets.BeginReceive(m_Data, 0, m_Data.Length, SocketFlags.None, OnReceive, null);
        }
    }

    public void Send(int id, byte[] body)
    {
        byte[] head = BitConverter.GetBytes(id);
        byte[] len = BitConverter.GetBytes((ushort)(head.Length + body.Length));
        byte[] data = new byte[0];
        data = data.Concat(len).ToArray();
        data = data.Concat(head).ToArray();
        data = data.Concat(body).ToArray();
        m_Sockets.BeginSend(data, 0, data.Length, SocketFlags.None, OnSend, null);
    }

    private void OnSend(IAsyncResult ar)
    {
        int len = m_Sockets.EndSend(ar);
        Debug.Log("�ͻ��˷��ͳ���:" + len);
    }
    public void Updata()
    {
        if (m_que.Count > 0)
        {
            byte[] oneData = m_que.Dequeue();
            int id = BitConverter.ToInt32(oneData, 0);
            byte[] body = new byte[oneData.Length - 4];
            Buffer.BlockCopy(oneData, 4, body, 0, body.Length);
            MessageCenter.GetInstance.BroadCast(id, body);
        }
    }
}