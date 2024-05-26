using System;
using System.Collections.Generic;
using UnityEngine;

namespace RDG.Util.Scripts.Motor {

  
  [CreateAssetMenu(menuName = "RDG/Util/Motor")]
  public class MotorSo: ScriptableObject {

    [SerializeField] private MotorGlobalConfig config;

    private readonly Dictionary<string, Ground> nameToGround = new Dictionary<string, Ground>();

    public void AddGround(Ground ground) {
      ground.Collider.gameObject.layer = GroundLayer;
      nameToGround.Add(ground.Collider.name, ground);
    }
    
    public int GroundLayer => LayerMask.NameToLayer(config.groundLayerName);
    public int GroundLayerMask => LayerMask.GetMask(config.groundLayerName);
    
    public void RemoveGround(Ground ground) {
      nameToGround.Remove(ground.Collider.name);
    }

    public Ground GetGround(Collider collider) {
      if (collider == null) {
        return null;
      }
      return nameToGround.TryGetValue(collider.name, out var ground) ? ground : null;
    }
    
    public MotorGlobalConfig Config => config;
  }
}
