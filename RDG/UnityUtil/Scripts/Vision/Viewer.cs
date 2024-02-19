using System.Collections.Generic;
using UnityEngine;


namespace RDG.UnityUtil.Scripts.Vision {
  public interface Viewer {
    public IEnumerable<GameObject> Visible { get; }
  }
}