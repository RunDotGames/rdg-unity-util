using System.Collections.Generic;
using UnityEngine;

namespace RDG.UnityUtil {
  
  [AddComponentMenu("RDG/Util/Cursor")]
  public class CursorBeh : MonoBehaviour {

    public CursorSo cursor;

    public void Start() {
      cursor.Reset();
    }

    public void Update() {
      if (Input.GetMouseButtonDown(0)) {
        cursor.HandleDown();
      }
      if (Input.GetMouseButtonUp(0)) {
        cursor.HandleUp();
      }
    }
    
  }
}
