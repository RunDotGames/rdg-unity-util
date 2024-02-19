using System;
using System.Collections.Generic;
using UnityEngine;


namespace RDG.UnityUtil.Scripts.Vision {
  
  [Serializable]
  public class SenseConfig {
    public string senseLayerName;
  }
  
  [CreateAssetMenu(menuName = "RDG/Util/Vision")]
  public class VisionSo : ScriptableObject {

    [SerializeField] private SenseConfig config;

    private bool isEnabled;
    private int visionLayer;

    private readonly Dictionary<string, Visible> nameToVisible = new Dictionary<string, Visible>();
    public LayerMask VisionLayer => visionLayer;
    
    public void AddVisible(Visible visible) {
      visible.VisibleCollider.gameObject.layer = LayerMask.NameToLayer(config.senseLayerName);
      nameToVisible.Add(visible.VisibleCollider.name, visible);
    }
    
    public void RemoveVisible(Visible visible) {
      nameToVisible.Remove(visible.VisibleCollider.name);
    }

    public Visible GetVisible(Collider collider) {
      if (!isEnabled) {
        return null;
      }
      
      nameToVisible.TryGetValue(collider.name, out var visible);
      return visible;
    }
    
    public void Enable() {
      isEnabled = true;
      visionLayer = LayerMask.GetMask(config.senseLayerName);
    }

    public void Disable() {
      isEnabled = false;
      visionLayer = -1;
    }
  }
}
