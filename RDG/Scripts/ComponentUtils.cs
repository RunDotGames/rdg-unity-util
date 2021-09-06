using System;
using UnityEngine;

namespace RDG.UnityUtil {
  public static class ComponentUtils {
    
    public static TRequired GetRequiredComp<TRequired>(MonoBehaviour source) {
      var found = source.GetComponent<TRequired>();
      if (found == null) {
        throw new Exception($"{nameof(source)} requires same obj {nameof(TRequired)}");
      }
      return found;
    }
  }
}
