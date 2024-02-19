using System;

namespace RDG.UnityUtil.Scripts {
  public interface ReadableEventProp<out T> {

    public event Action<T> OnChange;
    public T Value { get; }
  }

  public interface MutableEventProp<T> : ReadableEventProp<T> {
    public void Set(T value);

    public void Release();
  }

  public class EventProp<T> : MutableEventProp<T> where T : IEquatable<T> {
    
    public event Action<T> OnChange;
    public T Value { get; private set; }

    public void Set(T value) {
      if (value == null && Value == null) {
        return;
      }
      if (Value != null && Value.Equals(value)) {
        return;
      }
      Value = value;
      OnChange?.Invoke(value);
    }

    public void Release() {
      OnChange = null;
    }
  }
  
  
  
}
