using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Barracuda.UISystem;

namespace Barracuda.Editor
{
	[CustomEditor(typeof(MonoStreamer))]
	public class MonoStreamerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var streamees = serializedObject.FindProperty("streamees");

			if (streamees.arraySize == 0) {
				EditorGUILayout.HelpBox("No Streamees are registered!", MessageType.Warning);
			} else {
				for (var i = 0;; i++) {
					if (i >= streamees.arraySize) {
						break;
					}
					var streamee = streamees.GetArrayElementAtIndex(i);
					EditorGUILayout.BeginHorizontal();

					if (GUILayout.Button("Remove")) {
						streamees.DeleteArrayElementAtIndex(i);
						continue;
					}

					streamee.objectReferenceValue = EditorGUILayout.ObjectField(
						streamee == null ? null : streamee.objectReferenceValue,
						typeof(Tween),
						allowSceneObjects: true);

					if (streamee.objectReferenceValue == null) {
						EditorGUILayout.EndHorizontal();
						continue;
					}

					EditorGUILayout.EndHorizontal();

					var tween = (Tween)streamee.objectReferenceValue;
					EditorGUILayout.BeginHorizontal();

					EditorGUILayout.CurveField(tween.Easing, GUILayout.Height(72));

					EditorGUILayout.BeginVertical();
					EditorGUILayout.EnumPopup(tween.Target);
					EditorGUILayout.FloatField(tween.Value);
					EditorGUILayout.EnumPopup(tween.PropertyType);
					EditorGUILayout.FloatField(tween.Duration);
					EditorGUILayout.EndVertical();

					EditorGUILayout.EndHorizontal();
				}
			}

			if (GUILayout.Button("Add Tween")) {
				streamees.InsertArrayElementAtIndex(streamees.arraySize);
			}

			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}
}