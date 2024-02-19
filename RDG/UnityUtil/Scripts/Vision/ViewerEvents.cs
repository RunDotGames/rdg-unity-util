using System;
using UnityEngine;
using UnityEngine.Events;


namespace RDG.UnityUtil.Scripts.Vision {

  [Serializable]
  public class VisionEvents {
    public UnityEvent<GameObject, bool> onVisibilityChanged;

    public void Release() {
      onVisibilityChanged.RemoveAllListeners();
    }
  }
}
