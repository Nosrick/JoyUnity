#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;

namespace DTValidator.Internal {
	public static class UnityEngineObjectExtensions {
		// PRAGMA MARK - Public Interface
		public static int GetLocalId(this UnityEngine.Object unityObject) {
			if (unityObject == null) {
				return -1;
			}

			SerializedObject serializedObject = new SerializedObject(unityObject);
			inspectorModeInfo_.SetValue(serializedObject, InspectorMode.Debug, null);

			SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");
			return localIdProp.intValue;
		}


		// PRAGMA MARK - Internal
		private static readonly PropertyInfo inspectorModeInfo_ = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
	}
}
#endif
