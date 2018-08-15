using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class LightLevelHelper
    {
        private static float MAX_LIGHT = 16.0f;

        public static float Normalise(int light)
        {
            return light / MAX_LIGHT;
        }

        public static Color GetColour(int light)
        {
            float normalised = Normalise(light);
            return new Color(normalised, normalised, normalised, 1.0f - normalised);
        }
    }
}
