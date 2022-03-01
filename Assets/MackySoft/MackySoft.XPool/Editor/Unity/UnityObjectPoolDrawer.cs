using UnityEngine;
using UnityEditor;
using MackySoft.XPool.Unity.ObjectModel;

namespace MackySoft.XPool.Unity {

	[CustomPropertyDrawer(typeof(UnityObjectPool<>),true)]
	[CustomPropertyDrawer(typeof(UnityObjectPoolBase<>),true)]
	public class UnityObjectPoolDrawer : PropertyDrawer {

		public override void OnGUI (Rect position,SerializedProperty property,GUIContent label) {
			EditorGUI.BeginProperty(position,label,property);
			EditorGUI.BeginDisabledGroup(Application.isPlaying);

			SerializedProperty original = property.FindPropertyRelative("m_Original");
			SerializedProperty capacity = property.FindPropertyRelative("m_Capacity");

			// Draw prefix label
			Rect prefixLabelPosition = position;
			prefixLabelPosition.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.PrefixLabel(position,label);

			Rect propertyPosition = EditorGUI.IndentedRect(prefixLabelPosition);
			propertyPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			using (new EditorGUI.IndentLevelScope()) {
				// Draw original property
				EditorGUI.PropertyField(propertyPosition,original);

				// Draw capacity property
				propertyPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

				EditorGUI.BeginChangeCheck();
				int capacityValue = Mathf.Max(0,EditorGUI.IntField(propertyPosition,new GUIContent(capacity.displayName,capacity.tooltip),capacity.intValue));
				if (EditorGUI.EndChangeCheck() && (capacity.intValue != capacityValue)) {
					capacity.intValue = capacityValue;
				}

				if (property.Copy().CountRemaining() > 2) {
					propertyPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
					foreach (var child in SerializedPropertyUtility.GetVisibleChildren(property)) {
						if (child.name == "m_Original" || child.name == "m_Capacity") {
							continue;
						}
						float height = EditorGUI.GetPropertyHeight(child);
						propertyPosition.height = height;
						EditorGUI.PropertyField(propertyPosition,child);
						propertyPosition.y += height + EditorGUIUtility.standardVerticalSpacing;
					}
				}
			}

			EditorGUI.EndDisabledGroup();
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight (SerializedProperty property,GUIContent label) {
			float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			foreach (var child in SerializedPropertyUtility.GetVisibleChildren(property)) {
				height += EditorGUI.GetPropertyHeight(child) + EditorGUIUtility.standardVerticalSpacing;
			}
			return height;
		}

	}
}