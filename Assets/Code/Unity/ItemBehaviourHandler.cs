using System.Collections;
using System.Collections.Generic;
using DevionGames.InventorySystem;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Unity;
using UnityEngine;

public class ItemBehaviourHandler : MonoBehaviourHandler
{
    protected ItemCollection m_Items;

    public void Awake()
    {
        m_Items = this.GetComponent<ItemCollection>();
    }
    
    public override void AttachJoyObject(JoyObject joyObject)
    {
        base.AttachJoyObject(joyObject);

        if (joyObject is ItemInstance itemInstance)
        {
            m_Items.Add(itemInstance.Item);
        }
    }
}
