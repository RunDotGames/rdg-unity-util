using System;
using System.Collections.Generic;
using RDG.UnityUtil.Scripts.Filter;
using RDG.UnityUtil.Scripts.Vision;
using UnityEngine;
using UnityEngine.Serialization;

namespace RDG.UnityUtil.Scripts.Sense {
  
  [Serializable]
  public class ViewerBubbleConfig {
    [FormerlySerializedAs("distance")] public float radius;
    public float height;
  }
  
  public class ViewerBubbleBeh : MonoBehaviour, Viewer {

    [SerializeField] private VisionSo vision;
    [SerializeField] private ViewerBubbleConfig config;
    [SerializeField] private VisionEvents events;
    [SerializeField] private GameObject filterRoot;
    
    private HashSet<GameObject> visionCache = new();
    private GameObjectFilter[] filters = {};
    
    public float ViewFactor { get; set; }

    public void Awake() {
      ViewFactor = 1.0f;
      filterRoot = filterRoot == null ? gameObject : filterRoot;
      filters = filterRoot.GetComponentsInChildren<GameObjectFilter>();
    }
    public void OnDisable() {
      foreach (var visible in visionCache) {
        events.onVisibilityChanged.Invoke(visible, false);
      }
      visionCache.Clear();
    }

    public void OnDestroy() {
      events.Release();
    }
    public void FixedUpdate() {
      var hitCount = Physics.OverlapSphereNonAlloc(
        transform.position,
        config.radius * ViewFactor,
        CacheUtil.Colliders,
        vision.VisionLayer
      );
      HashSet<GameObject> visibleNow = new();
      for (var index = 0; index < hitCount; index++) {
        var visible = vision.GetVisible(CacheUtil.Colliders[index]);
        if (visible == null) {
          continue;
        }
        var heightDiff = visible.Root.transform.position.y - transform.position.y;
        if ( Math.Abs(heightDiff) > config.height){
          continue;
        }

        if (!CheckFilters(visible.Root.gameObject)) {
          continue;
        }
        
        visibleNow.Add(visible.Root.gameObject);
        if (!visionCache.Contains(visible.Root.gameObject)) {
          events.onVisibilityChanged.Invoke(visible.Root.gameObject, true);
          continue;
        }
        
        visionCache.Remove(visible.Root.gameObject);
      }

      foreach (var visible in visionCache) {
        events.onVisibilityChanged.Invoke(visible, false);
      }
      visionCache = visibleNow;
    }

    private bool CheckFilters(GameObject go) {
      foreach (var filter in filters) {
        if (!filter.Check(go)) {
          return false;
        }
      }
      return true;
    }
    public IEnumerable<GameObject> Visible => visionCache;

  }
}
