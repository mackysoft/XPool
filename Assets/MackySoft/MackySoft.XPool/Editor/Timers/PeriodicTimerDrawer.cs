using UnityEngine;
using UnityEditor;

namespace MackySoft.XPool.Timers {

	[CustomPropertyDrawer(typeof(PeriodicTimer))]
	public class PeriodicTimerDrawer : PropertyDrawer {

		public override void OnGUI (Rect position,SerializedProperty property,GUIContent label) {
			EditorGUI.BeginProperty(position,label,property);

			SerializedProperty interval = property.FindPropertyRelative("m_Interval");

			EditorGUI.BeginChangeCheck();

			float intervalValue = Mathf.Max(0.01f,EditorGUI.FloatField(position,label,interval.floatValue));

			if (EditorGUI.EndChangeCheck() && (interval.floatValue != intervalValue)) {
				interval.floatValue = intervalValue;
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight (SerializedProperty property,GUIContent label) {
			return EditorGUIUtility.singleLineHeight;
		}
	}
}