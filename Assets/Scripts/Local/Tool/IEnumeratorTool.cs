//***********************************************************
// 描述：这是一个功能代码
// 作者：阿福 
// 创建时间：2023-12-28 10:01:29
// 版 本：1.0
//***********************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class IEnumeratorTool : MonoBehaviour
{
    private static IEnumeratorTool inst;
    private void Awake()
    {
        inst = this;
    }


    /// <summary>
    /// 压入的action任务
    /// </summary>
    public class ActionTask
    {
        public Action willDoAction;
        public Action callBackAction;
    }

    /// <summary>
    /// 任务队列
    /// </summary>
    static Queue<ActionTask> actionTaskQueue = new Queue<ActionTask>();

    /// <summary>
    /// 执行任务
    /// </summary>
    /// <param name="action"></param>
    /// <param name="callBack"></param>
    static public void ExecAction(Action action, Action callBack = null)
    {
        var task = new ActionTask()
        {
            willDoAction = action,
            callBackAction = callBack
        };


        actionTaskQueue.Enqueue(task);
    }


    /// <summary>
    /// 任务队列
    /// </summary>
    static Queue<ActionTask> actionTaskQueueImmediately = new Queue<ActionTask>();

    /// <summary>
    /// 立即执行
    /// </summary>
    static public void ExecActionImmediately(Action action, Action callBack = null)
    {
        var task = new ActionTask()
        {
            willDoAction = action,
            callBackAction = callBack
        };


        actionTaskQueueImmediately.Enqueue(task);
    }

    //
    static Dictionary<int, IEnumerator> iEnumeratorDictionary = new Dictionary<int, IEnumerator>();//所有iEnumerator
    static Dictionary<int, Coroutine> coroutineDictionary = new Dictionary<int, Coroutine>();
    static List<int> IEnumeratorQueue = new List<int>();//开始协程队列
    static int counter = -1;

    static public bool GetCoroutineRunningStateById(int Id) //false 协程结束 ,true协程在执行
    {
        return iEnumeratorDictionary.ContainsKey(Id) || coroutineDictionary.ContainsKey(Id);
    }


    static public new int StartCoroutine(IEnumerator ie)
    {
        counter++;
        IEnumeratorQueue.Add(counter);
        iEnumeratorDictionary[counter] = ie;
        return counter;
    }

    static public int StartCoroutineImmediately(IEnumerator ie)
    {
        counter++;
        iEnumeratorDictionary[counter] = ie;
        ((MonoBehaviour)inst).StartCoroutine(ie);
        return counter;
    }

    static Queue<int> stopIEIdQueue = new Queue<int>();
    static public void StopCoroutine(int id)
    {
        Coroutine coroutine = null;
        if (coroutineDictionary.TryGetValue(id, out coroutine))
        {
            inst.StopCoroutine(coroutine);
            coroutineDictionary.Remove(id);
        }
        else if (iEnumeratorDictionary.ContainsKey(id))
        {
            // 如果协程还未启动，可以从队列中移除
            iEnumeratorDictionary.Remove(id);
            IEnumeratorQueue.Remove(id);
        }
        else
        {
            Debug.LogErrorFormat("此id协程不存在,无法停止:{0}", id);
        }
    }
    //static public void StopCoroutine(int id)
    //{
    //    if (!coroutineDictionary.ContainsKey(id))
    //    {
    //        Debug.LogErrorFormat("此id协程不存在,无法停止:{0}", id);
    //        return;
    //    }
    //    stopIEIdQueue.Enqueue(id);


    public static void StopCoroutineImmediately(int id)
    {
        Coroutine coroutine = null;
        if (IEnumeratorQueue.Contains(id))
        {
            IEnumeratorQueue.Remove(id);
            if (iEnumeratorDictionary.ContainsKey(id))
            {
                iEnumeratorDictionary.Remove(id);

            }
        }
        else if (coroutineDictionary.TryGetValue(id, out coroutine))
        {
            inst.StopCoroutine(coroutine);
            //
            coroutineDictionary.Remove(id);

        }
        else
        {
            Debug.LogErrorFormat("此id协程不存在,无法停止:{0}", id);
        }

    }

    static private bool isStopAllCroutine = false;

    /// <summary>
    /// 停止携程
    /// </summary>
    static public void StopAllCroutine()
    {
        isStopAllCroutine = true;
    }

    #region Tools

    /// <summary>
    /// 等待一段时间后执行
    /// </summary>
    /// <param name="f"></param>
    /// <param name="action"></param>
    static public void WaitingForExec(float f, Action action)
    {
        StartCoroutine(IE_WaitingForExec(f, action));
    }

    static private IEnumerator IE_WaitingForExec(float f, Action action)
    {
        yield return new WaitForSecondsRealtime(f);
        if (action != null)
        {
            action();
        }
    }

    #endregion

    /// <summary>
    /// 主循环
    /// </summary>
    void Update()
    {
        //停止所有携程
        if (isStopAllCroutine)
        {
            StopAllCoroutines();
            isStopAllCroutine = false;
        }

        //优先停止携程
        while (stopIEIdQueue.Count > 0)
        {
            var id = stopIEIdQueue.Dequeue();
            Coroutine coroutine = null;
            if (coroutineDictionary.TryGetValue(id, out coroutine))
            {
                base.StopCoroutine(coroutine);
                //
                coroutineDictionary.Remove(id);
            }
            else
            {
                Debug.LogErrorFormat("此id协程不存在,无法停止:{0}", id);
            }
        }

        //携程循环
        while (IEnumeratorQueue.Count > 0)
        {
            var id = IEnumeratorQueue[0];
            IEnumeratorQueue.RemoveAt(0);
            //取出携程
            var ie = iEnumeratorDictionary[id];
            iEnumeratorDictionary.Remove(id);
            //执行携程
            var coroutine = base.StartCoroutine(ie);
            //存入coroutine
            coroutineDictionary[id] = coroutine;
        }

        //主线程循环 立即执行
        while (actionTaskQueueImmediately.Count > 0)
        {
            var task = actionTaskQueueImmediately.Dequeue();
            task.willDoAction();
            if (task.callBackAction != null)
            {
                task.callBackAction();
            }
        }

        //主线程循环
        if (actionTaskQueue.Count > 0)
        {
            var task = actionTaskQueue.Dequeue();
            task.willDoAction();
            if (task.callBackAction != null)
            {
                task.callBackAction();
            }
        }
    }
}