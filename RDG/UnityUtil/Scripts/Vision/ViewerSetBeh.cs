using System.Collections.Generic;
using UnityEngine;

namespace RDG.UnityUtil.Scripts.Vision {
  public class ViewerSetBeh : MonoBehaviour, Viewer {

    private readonly HashSet<GameObject> visible = new();
    public IEnumerable<GameObject> Visible => visible;

    public void Modify(GameObject item, bool isAdded) {
      switch (isAdded) {
        case true when !visible.Contains(item):
          visible.Add(item);
          break;
        case false when visible.Contains(item):
          visible.Remove(item);
          break;
      }
    }
    
  }
}
