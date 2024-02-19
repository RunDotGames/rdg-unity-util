using System;
using UnityEngine.Events;

namespace RDG.Util.Scripts.Motor {
  [Serializable]
  public class MotorEvents {
    public UnityEvent onDashStart;
    public UnityEvent onDashStop;
    public UnityEvent onGrounded;
    public UnityEvent onJump;
    public void Release() {
      onGrounded.RemoveAllListeners();
      onJump.RemoveAllListeners();
      onDashStart.RemoveAllListeners();
      onDashStop.RemoveAllListeners();
    }
  }
}