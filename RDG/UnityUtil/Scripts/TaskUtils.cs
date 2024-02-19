using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RDG.UnityUtil.Scripts {
  public static class TaskUtils {

    public static Task WaitCoroutine(MonoBehaviour parent, float duration) {
      return WaitCoroutine(parent, duration, out _);
    }
    public static Task WaitCoroutine(MonoBehaviour parent, float duration, out CancellationTokenSource cancel) {
      cancel = new CancellationTokenSource();
      var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();  
      var source = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
      parent.StartCoroutine(_WaitCoroutine(source, duration));
      return source.Task.ContinueWith(
        _ => { },
        cancel.Token,
        TaskContinuationOptions.OnlyOnRanToCompletion & TaskContinuationOptions.ExecuteSynchronously,taskScheduler
      );
    }

    private static IEnumerator<YieldInstruction> _WaitCoroutine(TaskCompletionSource<bool> source, float duration) {
      yield return new WaitForSeconds(duration);
      if (!source.Task.IsCanceled) {
        source.SetResult(true);  
      }
    }
    
  }
}
