using JoyLib.Code.Entities.AI.LOS.Providers;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class LightLevelHelper
    {
        private static float Normalise(int light, int minValue = 0, int maxValue = GlobalConstants.MAX_LIGHT)
        {
            int adjusted = light == 0 ? 1 : light;
            return (adjusted - minValue) / (float)(maxValue - minValue);
        }

        public static Color GetColour(int light, IVision vision)
        {
            Color colour;
            if (light >= vision.MinimumLightLevel && light <= vision.MaximumLightLevel)
            {
                float hue = Normalise(light, vision.MinimumLightLevel, vision.MaximumLightLevel);
                float alpha = 1.0f - hue;
                Color displayColour = Color.Lerp(vision.DarkColour, vision.LightColour, hue);
                displayColour.a = alpha;
                colour = displayColour;
            }
            else if (light > vision.MaximumLightLevel)
            {
                colour = vision.LightColour;
            }
            else
            {
                colour = vision.DarkColour;
            }
            
            return colour;
        }
    }
}
