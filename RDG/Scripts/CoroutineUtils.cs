using UnityEngine;

namespace RDG.UnityUtil {
  
  public static class CoroutineUtils {
    public static readonly YieldInstruction EndOfFrame = new WaitForEndOfFrame();    
  }
}
