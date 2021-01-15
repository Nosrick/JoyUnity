using System.Collections.Generic;
using JoyLib.Code.Graphics;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public interface IGUIManager
    {
        void Clear();
        
        void AddGUI(GUIData gui);

        void ToggleGUI(string name);

        void SetUIColours(IDictionary<string, Color> background, IDictionary<string, Color> cursor);

        GUIData OpenGUI(string name, bool bringToFront = false);

        void CloseGUI(string activeName);

        void BringToFront(string name);

        void CloseAllOtherGUIs(string activeName = "");

        void CloseAllGUIs();

        bool RemovesControl();

        bool RemoveActiveGUI(string name);

        GUIData GetGUI(string name);

        bool IsActive(string name);

        bool AreAnyOpen();

        ISpriteState Background { get; }
        ISpriteState Cursor { get; }

        TMP_FontAsset FontToUse { get; }
        
        IDictionary<string, Color> CursorColours { get; }
        
        IDictionary<string, Color> BackgroundColours { get; }
        
        float MinFontSize { get; }
        float MaxFontSize { get; }
    }
}