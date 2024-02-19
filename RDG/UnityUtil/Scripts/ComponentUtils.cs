using System;
using UnityEngine;

namespace RDG.UnityUtil.Scripts {
  public static class ComponentUtils {
    
    public static TRequired GetRequiredComp<TRequired>(MonoBehaviour source) {
      var found = source.GetComponent<TRequired>();
      if (found == null) {
        throw new Exception($"{nameof(source)} requires {nameof(TRequired)}");
      }
      return found;
    }
    
    public static TRequired GetRequiredComp<TRequired>(MonoBehaviour source, TRequired existing) {
      return existing == null ? GetRequiredComp<TRequired>(source) : existing;
    }

    public static TRequired GetRequiredCompInChildren<TRequired>(MonoBehaviour source) {
      var found = source.GetComponentInChildren<TRequired>();
      if (found == null) {
        throw new Exception($"{nameof(source)} requires child {nameof(TRequired)}");
      }
      return found;
    }
    
    public static TRequired GetRequiredCompInChildren<TRequired>(MonoBehaviour source, TRequired existing) {
      return existing == null ? GetRequiredCompInChildren<TRequired>(source) : existing;
    }
  }
}
