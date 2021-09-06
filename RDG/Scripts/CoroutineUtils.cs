using UnityEngine;

namespace RD.UnityUtil {
  
  public static class CoroutineUtils {
    public static readonly YieldInstruction EndOfFrame = new WaitForEndOfFrame();    
  }
}
