using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RDG.Util.Scripts.Motor {

  public interface Ground {
    public Guid Id { get; }
    public Collider Collider { get; }
  }
  
  public class GroundBeh : MonoBehaviour, Ground {

    public MotorSo motors;
    
    public void Awake() {
      Id = Guid.NewGuid();
      Collider = GetComponentInChildren<Collider>();
      Collider.name = "ground: " + Id;
      var body = GetComponentInChildren<Rigidbody>();
      
      if (body != null) {
        body.isKinematic = true;
      } 
      motors.AddGround(this);
    }

    public void OnDestroy() {
      motors.RemoveGround(this);
    }

    public Guid Id { get; private set; }
    public Collider Collider { get; private set; }
  }
}
