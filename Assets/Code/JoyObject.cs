using UnityEngine;
using JoyLib.Code.Helpers;
using System;
using System.Collections.Generic;
using JoyLib.Code.Managers;

[Serializable]
public class JoyObject
{
    protected int m_HitPoints;
    protected int m_HitPointsRemaining;
    
    protected Vector2Int m_WorldPosition;

    [NonSerialized]
    protected Sprite[] m_Icons;

    protected int m_LastIcon;

    protected int m_FramesSinceLastChange;

    protected const int FRAMES_PER_SECOND = 30;

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
    public JoyObject(string name, int hitPoints, Vector2Int position, List<Sprite> sprites, string baseType, bool isAnimated, bool isWall = false, bool isDestructible = true)
    {
        this.JoyName = name;
        this.GUID = GUIDManager.AssignGUID();

        this.m_HitPoints = hitPoints;

        this.m_WorldPosition = position;
        this.Move(this.m_WorldPosition);

        this.m_Icons = sprites.ToArray();

        this.BaseType = baseType;
        this.IsAnimated = isAnimated;
        this.IsWall = isWall;
        this.IsDestructible = isDestructible;

        //If it's not animated, select a random icon to represent it
        if (!this.IsAnimated && sprites != null)
        {
            this.ChosenIcon = RNG.Roll(0, sprites.Count - 1);
        }
        else
        {
            this.ChosenIcon = 0;
        }

        this.m_LastIcon = 0;
        this.m_FramesSinceLastChange = 0;
    }

    ~JoyObject()
    {
        GUIDManager.ReleaseGUID(this.GUID);
    }

    public void Move(Vector2Int newPosition)
    {
        m_WorldPosition = newPosition;
    }

    public virtual void DamageMe(int value)
    {
        m_HitPointsRemaining = System.Math.Max(0, m_HitPointsRemaining - value);
    }

    public virtual void HealMe(int value)
    {
        m_HitPointsRemaining = System.Math.Min(m_HitPoints, m_HitPointsRemaining + value);
    }

    public void SetIcons(Texture2D[] textures)
    {
        m_Icons = new Sprite[textures.Length];
        for(int i = 0; i < textures.Length; i++)
        {
            m_Icons[i] = Sprite.Create(textures[i], new Rect(0, 0, 16, 16), Vector2.zero, 16);
        }
    }

    // Use this for initialization
    void Start ()
    {
		
	}

    // Update is called once per frame
    public virtual void Update ()
    {
        m_FramesSinceLastChange += 1;

        if(IsAnimated == false)
        {
            return;
        }

        if(m_FramesSinceLastChange == FRAMES_PER_SECOND)
        {
            ChosenIcon += 1;
            ChosenIcon %= m_Icons.Length;

            m_FramesSinceLastChange = 0;
        }
	}

    public string BaseType
    {
        get;
        protected set;
    }
    
    public Sprite Icon
    {
        get
        {
            return m_Icons[ChosenIcon];
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
            return m_HitPointsRemaining;
        }
    }

    public int HitPoints
    {
        get
        {
            return m_HitPoints;
        }
    }

    public bool Alive
    {
        get
        {
            return m_HitPointsRemaining > 0;
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
        get;
        protected set;
    }

    protected int ChosenIcon
    {
        get;
        set;
    }

    public bool IsWall
    {
        get;
        protected set;
    }

    public bool IsDestructible
    {
        get;
        protected set;
    }
}
