using System;

namespace RDG.Util.Scripts.Motor {
  
  [Serializable]
  public class MotorConfig {
    public float moveSpeed = 10.0f;
    public float jumpSpeed = 10.0f;
    public float rotationSpeed = 10.0f;
    public float radius = 1.0f;
    public float height = 1.0f;
    public float dashDurationSeconds = 1.0f;
    public float dashFactor = 2.0f;
  }
}