using System;
using System.Reflection;

namespace DTValidator {
	public interface IValidationError {
		int ObjectLocalId {
			get;
		}

		Type ObjectType {
			get;
		}

		MemberInfo MemberInfo {
			get;
		}

		object ContextObject {
			get;
		}
	}
}
