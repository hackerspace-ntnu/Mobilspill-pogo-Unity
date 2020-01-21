using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class UtilityFunctions
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
}