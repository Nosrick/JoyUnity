using System;
using System.Reflection;
using DTValidator.ValidationErrors;
using UnityEngine;

namespace DTValidator.Internal {
	public static class ValidationErrorFactory {
		// PRAGMA MARK - Static Public Interface
		public static IValidationError Create(object obj, Type objectType, MemberInfo memberInfo, object contextObject) {
			Component objAsComponent = obj as Component;
			if (objAsComponent != null) {
				return new ComponentValidationError(objAsComponent, objectType, memberInfo, contextObject);
			} else {
				return new ObjectValidationError(obj, objectType, memberInfo, contextObject);
			}
		}

		public static IValidationError Create(object obj, Type objectType, MemberInfo memberInfo, object contextObject, int index) {
			Component objAsComponent = obj as Component;
			if (objAsComponent != null) {
				return new IndexedComponentValidationError(objAsComponent, objectType, memberInfo, contextObject, index);
			} else {
				return new IndexedObjectValidationError(obj, objectType, memberInfo, contextObject, index);
			}
		}

		public static IValidationError Create(GameObject gameObject, object contextObject) {
			return new MissingMonoScriptValidationError(gameObject, contextObject);
		}
	}
}
