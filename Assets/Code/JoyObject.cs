using JoyLib.Code.Entities.Statistics;
using JoyLib.Code;
using JoyLib.Code.Managers;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using System;
using System.Collections.Generic;
using JoyLib.Code.Collections;
using System.Linq;
using UnityEngine;

[Serializable]
public class JoyObject : IComparable
{
    protected BasicValueContainer<IDerivedValue> m_DerivedValues;
    protected Vector2Int m_WorldPosition;

    [NonSerialized]
    protected Sprite[] m_Icons;

    protected List<string> m_Tags;

    protected string m_Tileset;

    protected int m_LastIcon;

    protected int m_FramesSinceLastChange;

    protected List<IJoyAction> m_CachedActions;

    protected const int FRAMES_PER_SECOND = 30;

    public JoyObject()
    {}

    /// <summary>
    /// Creation of a JoyObject (MonoBehaviour) using a List of Sprites
    /// </summary>
    /// <param name="name"></param>
    /// <param name="hitPoints"></param>
    /// <param name="position"></param>
    /// <param name="sprites"></param>
    /// <param name="baseType"></param>
    /// <param name="isAnimated"></param>
    /// <param name="isWall"></param>
    public JoyObject(
        string name, 
        BasicValueContainer<IDerivedValue> derivedValues, 
        Vector2Int position, 
        string tileSet, 
        string[] actions,
        Sprite[] sprites, 
        params string[] tags)
    {
        this.m_CachedActions = new List<IJoyAction>();
        this.JoyName = name;
        this.GUID = GUIDManager.Instance.AssignGUID();

        this.m_DerivedValues = derivedValues;

        this.m_Tileset = tileSet;
        this.m_Tags = tags.ToList();

        this.m_WorldPosition = position;
        this.Move(this.m_WorldPosition);

        this.m_Icons = sprites;

        //If it's not animated, select a random icon to represent it
        if (!this.IsAnimated && sprites != null)
        {
            this.ChosenIcon = RNG.instance.Roll(0, sprites.Length - 1);
        }
        else
        {
            this.ChosenIcon = 0;
        }

        this.m_LastIcon = 0;
        this.m_FramesSinceLastChange = 0;

        foreach(string action in actions)
        {
            m_CachedActions.Add(ScriptingEngine.instance.FetchAction(action));
        }
    }

    public JoyObject(
        string name, 
        BasicValueContainer<IDerivedValue> derivedValues, 
        Vector2Int position, 
        string tileSet, 
        IJoyAction[] actions,
        Sprite[] sprites, 
        params string[] tags)
    {
        this.JoyName = name;
        this.GUID = GUIDManager.Instance.AssignGUID();

        this.m_DerivedValues = derivedValues;

        this.m_Tileset = tileSet;
        this.m_Tags = tags.ToList();

        this.m_WorldPosition = position;
        this.Move(this.m_WorldPosition);

        this.m_Icons = sprites;

        //If it's not animated, select a random icon to represent it
        if (!this.IsAnimated && sprites != null)
        {
            this.ChosenIcon = RNG.instance.Roll(0, sprites.Length - 1);
        }
        else
        {
            this.ChosenIcon = 0;
        }

        this.m_LastIcon = 0;
        this.m_FramesSinceLastChange = 0;

        this.m_CachedActions = new List<IJoyAction>(actions);
    }

    ~JoyObject()
    {
        GUIDManager.Instance.ReleaseGUID(this.GUID);
    }

    public bool AddTag(string tag)
    {
        if(m_Tags.Contains(tag, GlobalConstants.STRING_COMPARER) == false)
        {
            m_Tags.Add(tag);
            return true;
        }
        return false;
    }

    public bool RemoveTag(string tag)
    {
        if(m_Tags.Contains(tag, GlobalConstants.STRING_COMPARER) == true)
        {
            m_Tags.Remove(tag);
            return true;
        }
        return false;
    }

    public bool HasTag(string tag)
    {
        return m_Tags.Contains(tag, GlobalConstants.STRING_COMPARER);
    }

    public void Move(Vector2Int newPosition)
    {
        m_WorldPosition = newPosition;
    }

    public virtual int DamageMe(int value, string index = "hitpoints")
    {
        return m_DerivedValues[index].ModifyValue(value);
    }

    public virtual int HealMe(int value, string index = "hitpoints")
    {
        return m_DerivedValues[index].ModifyValue(value);
    }

    //Used for deserialisation
    public void SetIcons(Sprite[] sprites)
    {
        m_Icons = sprites;
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        m_FramesSinceLastChange += 1;

        if(IsAnimated == false)
        {
            return;
        }

        if (m_FramesSinceLastChange != FRAMES_PER_SECOND)
        {
            return;
        }
        
        ChosenIcon += 1;
        ChosenIcon %= m_Icons.Length;

        m_FramesSinceLastChange = 0;
    }

    public int CompareTo(object obj)
    {
        if(obj == null)
        {
            return 1;
        }

        JoyObject joyObject = obj as JoyObject;
        if(joyObject != null)
        {
            return this.GUID.CompareTo(joyObject.GUID);
        }
        else
        {
            throw new ArgumentException("Object is not a JoyObject");
        }
    }

    public override string ToString()
    {
        return "{ " + this.JoyName + " : " + this.GUID + "}";
    }
    
    public Sprite Icon
    {
        get
        {
            return m_Icons[ChosenIcon];
        }
    }

    public Sprite[] Icons
    {
        get
        {
            return m_Icons;
        }
    }

    public long GUID
    {
        get;
        protected set;
    }

    public string JoyName
    {
        get;
        protected set;
    }

    public int HitPointsRemaining
    {
        get
        {
            return m_DerivedValues["hitpoints"].Value;
        }
    }

    public int HitPoints
    {
        get
        {
            return m_DerivedValues["hitpoints"].Maximum;
        }
    }

    public bool Conscious
    {
        get
        {
            return HitPointsRemaining > 0;
        }
    }

    public bool Alive
    {
        get
        {
            return HitPointsRemaining > (HitPoints * (-1));
        }
    }

    public Vector2Int WorldPosition
    {
        get
        {
            return m_WorldPosition;
        }
    }

    public bool IsAnimated
    {
        get
        {
            return m_Tags.Contains("animated");
        }
    }

    protected int ChosenIcon
    {
        get;
        set;
    }

    public bool IsWall
    {
        get
        {
            return m_Tags.Contains("wall");
        }
    }

    public bool IsDestructible
    {
        get
        {
            return m_Tags.Contains("invulnerable") == false;
        }
    }

    public int TotalTags
    {
        get
        {
            return m_Tags.Count;
        }
    }

    public string Tileset
    {
        get
        {
            return m_Tileset;
        }
    }

    public List<string> Tags
    {
        get
        {
            return new List<string>(m_Tags);
        }
    }
}
