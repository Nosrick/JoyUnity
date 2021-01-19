using System.Collections.Generic;
using JoyLib.Code.Graphics;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public interface IGUIManager
    {
        void Clear();

        void FindGUIs();
        
        void AddGUI(GUIData gui);

        void ToggleGUI(string name);

        void SetupManagedComponents(GUIData gui);

        void SetUIColours(IDictionary<string, IDictionary<string, Color>> background,
            IDictionary<string, IDictionary<string, Color>> cursor,
            IDictionary<string, Color> mainFontColours, 
            bool recolour = true);

        void RecolourGUIs();

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

        IDictionary<string, ISpriteState> Backgrounds { get; }
        IDictionary<string, ISpriteState> Cursors { get; }

        IDictionary<string, TMP_FontAsset> FontsToUse { get; }
        
        IDictionary<string, IDictionary<string, Color>> CursorColours { get; }
        
        IDictionary<string, IDictionary<string, Color>> BackgroundColours { get; }
        
        float MinFontSize { get; }
        float MaxFontSize { get; }
    }
}