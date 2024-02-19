using System;
using RDG.UnityUtil.Scripts.Cursor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RDG.UnityUtil {
  public class CursorDemoBeh: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public CursorSo cursor;
    public CursorState state;
    
    private Action releaseCursor;
    
    public void OnPointerEnter(PointerEventData eventData) {
      releaseCursor = cursor.Push(state);
    }
    
    public void OnPointerExit(PointerEventData eventData) {
      releaseCursor?.Invoke();
      releaseCursor = null;
    }
  }
  
  
}
