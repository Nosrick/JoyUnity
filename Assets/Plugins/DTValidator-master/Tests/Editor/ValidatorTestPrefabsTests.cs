using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace DTValidator.Internal {
	public static class ValidatorTestPrefabsTests {
		[Test]
		public static void ValidatorTestBrokenPrefabs_All_ReturnErrors() {
			GameObject[] brokenPrefabs = Resources.LoadAll<GameObject>("DTValidatorTestBrokenPrefabs");
			foreach (GameObject prefab in brokenPrefabs) {
				IList<IValidationError> errors = Validator.Validate(prefab);
				Assert.That(errors, Is.Not.Null, string.Format("Prefab named: '{0}' does not return any validation errors!", prefab.name));
			}
		}

		[Test]
		public static void ValidatorTestNotBrokenPrefabs_All_ReturnErrors() {
			GameObject[] notBrokenPrefabs = Resources.LoadAll<GameObject>("DTValidatorTestNotBrokenPrefabs");
			foreach (GameObject prefab in notBrokenPrefabs) {
				IList<IValidationError> errors = Validator.Validate(prefab);
				Assert.That(errors, Is.Null, string.Format("Prefab named: '{0}' returns validation errors when it shouldn't!", prefab.name));
			}
		}
	}
}
