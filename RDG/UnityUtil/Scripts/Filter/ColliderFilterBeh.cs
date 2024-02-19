using System;
using UnityEngine;

namespace RDG.UnityUtil.Scripts.Filter {
  public class ColliderFilterBeh : MonoBehaviour, GameObjectFilter {

    [SerializeField] private int layerMask;

    public int LayerMask => layerMask;
    
    public bool Check(GameObject go) {
      var start = transform.position;
      var end = go.transform.position;
      Debug.DrawLine(start, end, Color.magenta);
      return !Physics.Linecast(start, end, layerMask);
    }
  }
}
