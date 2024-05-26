using System;
using RDG.UnityUtil.Scripts.Cursor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RDG.UnityUtil {
  public class CursorDemoBeh: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public CursorRegistrySo cursors;
    public CursorInteractionSo interaction;
    
    private Action releaseCursor;
    
    public void OnPointerEnter(PointerEventData eventData) {
      releaseCursor = cursors.Push(interaction);
    }
    
    public void OnPointerExit(PointerEventData eventData) {
      releaseCursor?.Invoke();
      releaseCursor = null;
    }
  }
  
  
}
