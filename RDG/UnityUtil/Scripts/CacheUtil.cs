using UnityEngine;

namespace RDG.UnityUtil.Scripts {
  

  
  public class CacheUtil {
    
    public const int ColliderCacheSize = 100;
    public const int RayCastHitCacheSize = 100;

    public static readonly Collider[] Colliders = new Collider[ColliderCacheSize];
    public static readonly RaycastHit[] RaycastHits = new RaycastHit[RayCastHitCacheSize];
  }
}
