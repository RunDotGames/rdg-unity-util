using UnityEngine;

namespace RDG.UnityUtil.Scripts.Cursor {
  
  
  public class CursorEventProducerBeh : MonoBehaviour {

    public CursorRegistrySo cursors;

    public void Start() {
      cursors.Reset();
    }

    public void Update() {
      if (Input.GetMouseButtonDown(0)) {
        cursors.HandleDown();
      }
      if (Input.GetMouseButtonUp(0)) {
        cursors.HandleUp();
      }
    }
    
  }
}
