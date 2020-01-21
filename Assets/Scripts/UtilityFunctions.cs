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

    public static Task LogErrorOrContinueWith(this Task task, Action<Task> continuationFunction)
    {
        return task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                Debug.LogWarningFormat("Task error: {0}", t.Exception);
            }
            else if (t.IsCanceled)
            {
                Debug.LogWarningFormat("Task cancelled: {0}", t.Exception);
            }
            else {
                continuationFunction.Invoke(t);
            }
        });
    }
    public static Task<TResult> LogErrorOrContinueWith<TResult>(this Task task, Func<Task, TResult> continuationFunction)
    {
        return task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                Debug.LogWarningFormat("Task error: {0}", t.Exception);
            }
            else if (t.IsCanceled)
            {
                Debug.LogWarningFormat("Task cancelled: {0}", t.Exception);
            }
            return continuationFunction.Invoke(t);
        });
    }

    public static Task<TNewResult> LogErrorOrContinueWith<TResult,TNewResult>(this Task<TResult> task,Func<Task<TResult>, TNewResult> continuationFunction)
    {
        return task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                Debug.LogWarningFormat("Task error: {0}", t.Exception);
            }
            else if (t.IsCanceled)
            {
                Debug.LogWarningFormat("Task cancelled: {0}", t.Exception);
            }
            return continuationFunction.Invoke(t);
        });
    }
}