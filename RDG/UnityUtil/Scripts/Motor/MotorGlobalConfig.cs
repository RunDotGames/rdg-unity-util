using System;
using UnityEngine;


namespace RDG.Util.Scripts.Motor {
  
  [Serializable]
  public class MotorGlobalConfig {
    
    [Tooltip("The name of the layer that blocks movement")]
    public string groundLayerName;
    
    [Tooltip("The distance from the base of the motor to check for ground")]
    public float groundCheckDistance = 0.05f;
    
    [Tooltip("The distance to back up from the base of the motor to check for ground (handles motor object stuck in ground)")]
    public float checkBackupDistance = 0.1f;
    
    [Tooltip("Draw Debug RayCast lines")]
    public bool drawDebug;
    
    [Tooltip("Rate of acceleration for falling motors")]
    public float fallRate = 5.0f;
    
    [Tooltip("Terminal velocity for falling motors")]
    public float fallMaxSpeed = 10;
    
    [Tooltip("How high of the base of the motor to check for walls")]
    public float wallHeightCheck = .01f;
    
    [Tooltip("The velocity curve for jumps")]
    public AnimationCurve jumpCurve;
  }
}