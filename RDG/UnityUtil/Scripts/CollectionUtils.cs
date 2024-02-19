using System.Collections.Generic;

namespace RDG.UnityUtil.Scripts {
  public static class CollectionUtils {
    
    public static IEnumerable<T> Once<T>(T value) {
      yield return value;
    }
  }
}
