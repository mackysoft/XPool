using System.Collections.Generic;
using UnityEditor;

namespace MackySoft.XPool.Unity {
	public static class SerializedPropertyUtility {

		public static IEnumerable<SerializedProperty> GetVisibleChildren (this SerializedProperty property) {
			SerializedProperty currentProperty = property.Copy();
			SerializedProperty nextSiblingProperty = property.Copy();
			nextSiblingProperty.NextVisible(false);

			if (currentProperty.NextVisible(true)) {
				do {
					if (SerializedProperty.EqualContents(currentProperty,nextSiblingProperty)) {
						break;
					}
					yield return currentProperty;
				}
				while (currentProperty.NextVisible(false));
			}
		}

	}
}