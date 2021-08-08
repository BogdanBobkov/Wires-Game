using System;
using System.Collections;
using UnityEngine;

public static class Tools
{
    /// <summary>
    ///     Call delegate by time with normalized time in parameter (time scaled)
    /// </summary>
    /// <param name="mn"></param>
    /// <param name="func">Delegate with parameter float [0..1]</param>
    /// <param name="time">Time to work</param>
    /// <param name="endFunc">Delegate after end</param>
    public static Coroutine InvokeDelegate(this MonoBehaviour mn, Action<float> func, float time, Action endFunc = null)
    {
        return mn.StartCoroutine(InvokeDelegateCor(func, time, endFunc));
    }

    private static IEnumerator InvokeDelegateCor(Action<float> func, float time, Action endFunc)
    {
        var timer = 0f;
        while (timer < time)
        {
            func(timer / time);
            yield return null;
            timer += Time.deltaTime;
        }

        func(1f);
        endFunc();
    }
}
