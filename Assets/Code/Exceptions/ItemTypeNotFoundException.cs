using System;

public class ItemTypeNotFoundException : Exception
{
    public string m_ItemType;

    public ItemTypeNotFoundException(string itemType, string message) :
        base(message)
    {
        m_ItemType = itemType;
    }
}