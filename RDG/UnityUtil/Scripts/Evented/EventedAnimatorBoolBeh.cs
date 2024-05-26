using UnityEngine;

namespace RDG.UnityUtil.Scripts.Evented {
  public class EventedAnimatorBoolBeh : MonoBehaviour {
    [SerializeField] private Animator anim;
    [SerializeField] private string propName;
    [SerializeField] private bool staticValue;
    
    public void Start() {
      if (anim != null) return;
      
      anim = GetComponentInChildren<Animator>();
    }

    public void HandleEventStatic() {
      anim.SetBool(propName, staticValue);
    }

    public void HandleEvent(bool dynamicValue) {
      anim.SetBool(propName, dynamicValue);
    }
  }
}
