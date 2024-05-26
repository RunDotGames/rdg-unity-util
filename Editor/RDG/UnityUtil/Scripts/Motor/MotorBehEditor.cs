using UnityEditor;
using UnityEngine;

namespace RDG.Util.Scripts.Motor.Editor {
  
  [CustomEditor(typeof(MotorBeh))]
  public class MotorBehEditor : UnityEditor.Editor {

    public override void OnInspectorGUI() {
      base.OnInspectorGUI();
      
      if (serializedObject.targetObject is not MotorBeh motorBeh) return;

      if (!EditorApplication.isPlaying) return;
      
      EditorGUILayout.BeginVertical(GUI.skin.box);
      GUILayout.TextArea($"Grounded: {motorBeh.IsGrounded}");
      GUILayout.TextArea($"Request Locked: {motorBeh.IsRequestLocked}");
      GUILayout.TextArea($"Move Dir: {motorBeh.LastMoveDirection}");
      EditorGUILayout.EndHorizontal();
    }

  }
}
