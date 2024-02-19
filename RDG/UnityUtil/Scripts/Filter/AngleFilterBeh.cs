using System;
using UnityEngine;

namespace RDG.UnityUtil.Scripts.Filter {
  public class AngleFilterBeh : MonoBehaviour, GameObjectFilter {

    public float maxAngle;
    public bool Check(GameObject go) {
      var myTransform = transform;
      var dir = go.transform.position - myTransform.position;
      dir.y = 0;
      var angle = Vector3.Angle(
        myTransform.forward,
        Vector3.Normalize(dir)
      );
      return Math.Abs(angle) < maxAngle;
    }
  }
}
