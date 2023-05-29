using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MyExices_IEnumerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var t1 = Test1();
            GameMgr.Instance.StartCoroutine(t1);
            while (true)
            {
                Thread.Sleep(Time.deltaMilliseconds);
                GameMgr.Instance.UpdateCoroutine();
            }
        }

        private static IEnumerator Test1()
        {
            Console.WriteLine("开始执行Test1的携程方法了");
            yield return new WaitForSeconds(3);//将现在的这个线程挂起3秒钟之后再去执行携程
            Console.WriteLine("等待3秒钟执行这个输出");
            yield return new WaitForSeconds(5);
            Console.WriteLine("等待5秒钟执行这个输出");
        }
    }
}

public class GameMgr
{
    static GameMgr _instance = null;
    public static GameMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameMgr();
            }
            return _instance;
        }
    }
    private LinkedList<IEnumerator> coroutineList = new LinkedList<IEnumerator>();//线程的集合
    public void StartCoroutine(IEnumerator ie)
    {
        coroutineList.AddLast(ie);
    }
    public void StopCoroutine(IEnumerator ie)
    {
        try
        {
            coroutineList.Remove(ie);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    public void UpdateCoroutine()
    {
        var node = coroutineList.First;
        while (node != null)
        {
            IEnumerator ie = node.Value;
            bool ret = true;
            if (ie.Current is IWait)
            {
                IWait wait = (IWait)ie.Current;
                if (wait.Tick())
                {
                    ret = ie.MoveNext();
                }
            }
            else
            {
                ret = ie.MoveNext();
            }
            if (!ret)
            {
                coroutineList.Remove(node);
            }
            node = node.Next;
        }
    }
}
public interface IWait
{
    bool Tick();
}
public class WaitForSeconds : IWait
{
    float _time = 0;
    public WaitForSeconds(float time)
    {
        _time = time;
    }
    public bool Tick()
    {
        _time -= Time.deltaTime;
        return _time <= 0;
    }
}
public class WaitForFrames : IWait
{
    int _frames = 0;
    public WaitForFrames(int frames)
    {
        _frames = frames;
    }
    public bool Tick()
    {
        _frames -= 1;
        return _frames <= 0;
    }
}
