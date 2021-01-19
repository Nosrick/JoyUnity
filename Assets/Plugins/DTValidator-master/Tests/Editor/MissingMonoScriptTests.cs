using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace DTValidator.Internal {
	public static class MissingMonoScriptTests {
		[Test]
		public static void RenamedComponentMissingMonoScript_ReturnsErrors() {
			GameObject renamedComponentPrefab = Resources.Load<GameObject>("DTValidatorTests/TestRenamedComponentPrefab");

			IList<IValidationError> errors = Validator.Validate(renamedComponentPrefab);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
