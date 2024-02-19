using UnityEngine;

namespace RDG.UnityUtil.Scripts.Faction {

  
  public class FactionedBeh : MonoBehaviour, Factioned {

    [SerializeField] private FactionSo faction;

    public FactionSo Faction => faction;
  }
}
