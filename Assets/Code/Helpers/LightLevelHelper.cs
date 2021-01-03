using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class LightLevelHelper
    {
        private static float Normalise(int light, int minValue = 0, int maxValue = GlobalConstants.MAX_LIGHT)
        {
            return (light - minValue) / (float)(maxValue - minValue);
        }

        public static Color GetColour(int light, int minValue, int maxValue)
        {
            Color colour;
            if (light >= minValue && light <= maxValue)
            {
                float hue = Normalise(light, minValue, maxValue);
                float alpha = 1.0f - hue;
                colour = new Color(hue, hue, hue, alpha);
            }
            else if (light > maxValue)
            {
                colour = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                colour = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            }
            
            return colour;
        }
    }
}
