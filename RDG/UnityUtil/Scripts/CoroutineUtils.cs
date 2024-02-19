using UnityEngine;

namespace RDG.UnityUtil.Scripts {
  
  public static class CoroutineUtils {
    public static readonly YieldInstruction EndOfFrame = new WaitForEndOfFrame();    
  }
}
