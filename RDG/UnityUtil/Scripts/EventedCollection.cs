using System;
using System.Collections.Generic;
using UnityEngine;

namespace RDG.UnityUtil.Scripts {
  public class EventedCollection<T> {

    private HashSet<T> items = new();

    public Action<T, bool> OnCollectionChange;
    
    public void HandleCollectionChange(T item, bool isAdded) {
      if (!isAdded) {
        items.Remove(item);
        OnCollectionChange?.Invoke(item, false);
        return;
      }

      items.Add(item);
      OnCollectionChange?.Invoke(item, true);
    }

    public void Release() {
      OnCollectionChange = null;
    }

    public IEnumerable<T> Items => items;
  }
}
