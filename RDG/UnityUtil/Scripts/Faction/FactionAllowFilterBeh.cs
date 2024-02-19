using System.Collections.Generic;
using RDG.UnityUtil.Scripts.Filter;
using UnityEngine;

namespace RDG.UnityUtil.Scripts.Faction {
  public class FactionAllowFilterBeh : MonoBehaviour, GameObjectFilter {

    public List<FactionSo> allowed = new();
    public bool Check(GameObject go) {
      var faction = go.GetComponentInChildren<FactionedBeh>();
      return faction != null && allowed.Contains(faction.Faction);
    }
  }
}
