using UnityEngine;


namespace RDG.UnityUtil.Scripts.Vision {
  public interface Visible {
    public Transform Root { get; }
    public Collider VisibleCollider { get; }
  }
}