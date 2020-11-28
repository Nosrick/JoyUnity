using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public interface IGUIManager
    {
        void AddGUI(
            GameObject gui,
            bool removesControl = true,
            bool closesOthers = false);

        void ToggleGUI(string name);

        void OpenGUI(string name);

        void CloseGUI(string activeName);

        void BringToFront(string name);

        void CloseAllOtherGUIs(string activeName = "");

        bool RemovesControl();

        GameObject GetGUI(string name);

        bool IsActive(string name);

        bool AreAnyOpen();
    }
}