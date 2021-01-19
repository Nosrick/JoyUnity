using UnityEngine;

namespace DTValidator.ValidationErrors {
	public interface IComponentValidationError : IValidationError {
		Component Component { get; }
	}
}
