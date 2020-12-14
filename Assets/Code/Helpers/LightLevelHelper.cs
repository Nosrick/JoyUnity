using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class LightLevelHelper
    {
        private static float Normalise(int light, int maxValue)
        {
            return (float)light / (float)maxValue;
        }

        public static Color GetColour(int light, int minValue, int maxValue)
        {
            float normalised = 0f;
            Color colour;
            if (light > minValue)
            {
                normalised = Normalise(light, maxValue);
                colour = new Color(normalised, normalised, normalised, 1.0f - normalised);
            }
            else if (light > maxValue)
            {
                colour = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                colour = Color.black;
            }
            
            return colour;
        }
    }
}
