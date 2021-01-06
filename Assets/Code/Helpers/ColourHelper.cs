using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class ColourHelper
    {
        public static Color ParseHTMLString(string data)
        {
            Color colour = Color.magenta;
            ColorUtility.TryParseHtmlString(data, out colour);
            return colour;
        }
    }
}