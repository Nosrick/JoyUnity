using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace DTValidator.Internal {
	public static class UnityComponentValidationTests {
		[Test]
		public static void MissingUnityComponentOutlet_ReturnsNoErrors() {
			GameObject gameObject = new GameObject();

			gameObject.AddComponent<UnityEngine.UI.Slider>();
			gameObject.AddComponent<UnityEngine.SpriteRenderer>();

			IList<IValidationError> errors = Validator.Validate(gameObject);
			Assert.That(errors, Is.Null);
		}
	}
}
