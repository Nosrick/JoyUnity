using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public interface IGUIManager
    {
        void AddGUI(GUIData gui);

        void ToggleGUI(string name);

        void SetUIColours(IDictionary<string, Color> background, IDictionary<string, Color> cursor);

        GUIData OpenGUI(string name, bool bringToFront = true);

        void CloseGUI(string activeName);

        void BringToFront(string name);

        void CloseAllOtherGUIs(string activeName = "");

        bool RemovesControl();

        bool RemoveActiveGUI(string name);

        GUIData GetGUI(string name);

        bool IsActive(string name);

        bool AreAnyOpen();
    }
}