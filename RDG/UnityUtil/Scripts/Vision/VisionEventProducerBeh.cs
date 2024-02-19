using System;
using UnityEngine;

namespace RDG.UnityUtil.Scripts.Vision {
  public class VisionEventProducerBeh : MonoBehaviour {

    [SerializeField] private VisionSo vision;
    public void OnEnable() {
      vision.Enable();
    }

    public void OnDisable() {
      vision.Disable();
    }
  }
}
