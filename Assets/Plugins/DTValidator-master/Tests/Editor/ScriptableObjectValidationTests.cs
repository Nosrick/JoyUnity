using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace DTValidator.Internal {
	public static class ScriptableObjectValidationTests {
		[Test]
		public static void MissingOutlet_ReturnsErrors() {
			OutletScriptableObject obj = ScriptableObject.CreateInstance<OutletScriptableObject>();
			obj.Outlet = null;

			IList<IValidationError> errors = Validator.Validate(obj);
			Assert.That(errors, Is.Not.Null);
			Assert.That(errors.Count, Is.EqualTo(1));
		}
	}
}
