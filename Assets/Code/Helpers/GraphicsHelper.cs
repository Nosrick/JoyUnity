using System;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Helpers
{
    public static class GraphicsHelper
    {
        public static Color ParseHTMLString(string data)
        {
            Color colour = Color.magenta;
            ColorUtility.TryParseHtmlString(data, out colour);
            return colour;
        }

        public static Image.Type ParseFillMethodString(string data)
        {
            return Enum.TryParse(data, true, out Image.Type fillType) ? fillType : Image.Type.Filled;
        }

        public static SpriteDrawMode ParseDrawModeString(string data)
        {
            return Enum.TryParse(data, true, out SpriteDrawMode drawMode) ? drawMode : SpriteDrawMode.Simple;
        }
    }
}