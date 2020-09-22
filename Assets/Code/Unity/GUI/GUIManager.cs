﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InventoryMaster.Scripts.Inventory;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIManager
    {
        private List<Tuple<GameObject, bool>> m_GUIs;
        private Tuple<GameObject, bool> m_ActiveGUI;

        public GUIManager()
        {
            m_GUIs = new List<Tuple<GameObject, bool>>();
            m_ActiveGUI = null;
        }

        public void AddGUI(GameObject gui, bool removesControl = true)
        {
            gui.SetActive(false);
            m_GUIs.Add(new Tuple<GameObject, bool>(gui, removesControl));
        }

        public void OpenGUI(string name)
        {
            foreach(Tuple<GameObject, bool> gui in m_GUIs)
            {
                if(gui.Item1.name == name)
                {
                    CloseGUI();
                    m_ActiveGUI = gui;
                    m_ActiveGUI.Item1.SetActive(true);
                    break;
                }
            }
        }

        public void CloseGUI()
        {
            if(m_ActiveGUI != null)
            {
                m_ActiveGUI.Item1.SetActive(false);
                m_ActiveGUI = null;
            }
        }

        public void OpenInventory()
        {
            GameObject inventoryGUI = m_GUIs.First(gui => gui.Item1.name.Equals("GUIInventory", StringComparison.OrdinalIgnoreCase)).Item1;
            Inventory inventory = inventoryGUI.GetComponentInChildren<Inventory>(true);
            inventory.openInventory();
        }

        public void CloseInventory()
        {
            GameObject inventoryGUI = m_GUIs.First(gui => gui.Item1.name.Equals("GUIInventory", StringComparison.OrdinalIgnoreCase)).Item1;
            Inventory inventory = inventoryGUI.GetComponentInChildren<Inventory>();
            inventory.closeInventory();
        }

        public bool RemovesControl()
        {
            return m_ActiveGUI.Item2;
        }

        public GameObject GetGUI(string name)
        {
            foreach(Tuple<GameObject, bool> gui in m_GUIs)
            {
                if(gui.Item1.name.Equals(name))
                {
                    return gui.Item1;
                }
            }

            throw new InvalidOperationException("Could not find GUI by name " + name);
        }
    }
}
