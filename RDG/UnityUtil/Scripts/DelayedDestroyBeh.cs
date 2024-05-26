using System.Threading.Tasks;
using UnityEngine;

namespace RDG.UnityUtil.Scripts {
  
  // Destroy a GameObject after a delay
  public class DelayedDestroyBeh : MonoBehaviour {

    [SerializeField] private float delaySeconds = 10;
    [SerializeField] private bool triggerOnAwake;

    public void Awake() {
      if (!triggerOnAwake) return;
      
      DestroyDelayed();
    }
    public void DestroyDelayed() {
      TaskUtils.WaitCoroutine(this, delaySeconds).ContinueWith((_) => {
        Destroy(gameObject);
      }, TaskContinuationOptions.ExecuteSynchronously);
    }
  }
}
