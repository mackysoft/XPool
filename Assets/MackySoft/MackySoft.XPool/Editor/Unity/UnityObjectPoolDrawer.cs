using UnityEngine;
using UnityEditor;
using MackySoft.XPool.Unity.ObjectModel;

namespace MackySoft.XPool.Unity {

	[CustomPropertyDrawer(typeof(UnityObjectPool<>),true)]
	[CustomPropertyDrawer(typeof(UnityObjectPoolBase<>),true)]
	public class UnityObjectPoolDrawer : PropertyDrawer {

		static readonly float kPropertyHeight = EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
		
		public override void OnGUI (Rect position,SerializedProperty property,GUIContent label) {
			EditorGUI.BeginProperty(position,label,property);
			EditorGUI.BeginDisabledGroup(Application.isPlaying);

			SerializedProperty original = property.FindPropertyRelative("m_Original");
			SerializedProperty capacity = property.FindPropertyRelative("m_Capacity");

			// Draw prefix label
			Rect prefixLabelPosition = position;
			prefixLabelPosition.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.PrefixLabel(position,label);

			using (new EditorGUI.IndentLevelScope()) {
				// Draw original property
				Rect originalPosition = prefixLabelPosition;
				originalPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				originalPosition = EditorGUI.IndentedRect(originalPosition);
				EditorGUI.PropertyField(originalPosition,original);

				// Draw capacity property
				Rect capacityPosition = originalPosition;
				capacityPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

				EditorGUI.BeginChangeCheck();
				int capacityValue = Mathf.Max(0,EditorGUI.IntField(capacityPosition,new GUIContent(capacity.displayName,capacity.tooltip),capacity.intValue));
				if (EditorGUI.EndChangeCheck() && (capacity.intValue != capacityValue)) {
					capacity.intValue = capacityValue;
				}
				
			}

			EditorGUI.EndDisabledGroup();
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight (SerializedProperty property,GUIContent label) {
			return kPropertyHeight;
		}

	}
}