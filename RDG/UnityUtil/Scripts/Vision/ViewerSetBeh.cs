using System.Collections.Generic;
using UnityEngine;

namespace RDG.UnityUtil.Scripts.Vision {
  public class ViewerSetBeh : MonoBehaviour, Viewer {

    private readonly EventedCollection<GameObject> visible = new();
    public IEnumerable<GameObject> Visible => visible.Items;

    public void HandleVisionChange(GameObject item, bool isAdded) {
      visible.HandleCollectionChange(item, isAdded);
    }
    
  }
}
