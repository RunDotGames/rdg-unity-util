using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RDG.UnityUtil.Scripts {
  public class ModifyCollectionEvent<T>{
    public HashSet<T> Added { get;  }
    public HashSet<T> Removed { get; }

    public ModifyCollectionEvent(T added, T removed) {
      Added = new HashSet<T>(added == null ? Enumerable.Empty<T>() : Enumerable.Repeat(added, 1));
      Removed = new HashSet<T>(removed == null ? Enumerable.Empty<T>() : Enumerable.Repeat(removed, 1));
    }
    public ModifyCollectionEvent(HashSet<T> added, HashSet<T> removed) {
      Added = added ?? new HashSet<T>();
      Removed = removed ?? new HashSet<T>();
    }
    
    public static void Fire(IEnumerable<T> prior, IEnumerable<T> updated, UnityEvent<ModifyCollectionEvent<T>> modifyEvent) {
      var added = updated.Except(prior).ToHashSet();
      var removed = prior.Except(updated).ToHashSet();
      if (added.Any() || removed.Any()) { 
        modifyEvent.Invoke(new ModifyCollectionEvent<T>(added, removed));
      }
    }
    
    public void Apply(ICollection<T> collection) {
      foreach (var item in Added) {
        collection.Add(item);
      }
      foreach (var item in Removed) {
        collection.Remove(item);
      }
    }
    
    public static void ApplyEventData<T2>(ModifyCollectionEvent<EventData<T2>> modifyEvent, ICollection<T2> collection) {
      foreach (var item in modifyEvent.Added) {
        collection.Add(item.Data);
      }
      foreach (var item in modifyEvent.Removed) {
        collection.Remove(item.Data);
      }
    }
  }
  
  public class EventData<T> {
    public GameObject GameObject {get; }
    public T Data { get; }

    public EventData(GameObject gameObject, T data) {
      GameObject = gameObject;
      Data = data;
    }
  }
  
  
}
