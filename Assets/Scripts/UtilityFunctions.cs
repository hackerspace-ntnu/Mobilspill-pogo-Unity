using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System;

public static class UtilityFunctions
{

    public static IEnumerator RunTaskAsCoroutine(Task task)
    {
        while (!task.IsCompleted) yield return null;

        if (task.IsFaulted)
        {
            Debug.LogWarningFormat("Task was faulted: {0}", task.Exception);
        }

        if (task.IsCanceled)
        {
            Debug.LogWarningFormat("Task was cancelled: {0}", task.Exception);
        }

        yield return task;
    }

    public static void RunTaskAndLogErrors(Task task)
    {
        Task.Run(() => task.ContinueWith(
            t => {
                if (t.IsFaulted)
                {
                    Debug.LogWarningFormat("Task was faulted: {0}", t.Exception);
                }

                if (t.IsCanceled)
                {
                    Debug.LogWarningFormat("Task was cancelled: {0}", t.Exception);
                }
            }
        ));

    }

    public static bool OnClickDown()
    {
        return (Application.isMobilePlatform && Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0);
    }
}