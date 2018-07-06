using UnityEngine;
using JoyLib.Code.Helpers;
using System;
using System.Collections.Generic;

[Serializable]
public class JoyObject : MonoBehaviour
{
    protected int m_HitPoints;
    protected int m_HitPointsRemaining;
    
    protected Vector2Int m_WorldPosition;

    protected Sprite[] m_Icons;

    protected int m_LastIcon;

    protected int m_FramesSinceLastChange;

    protected const int FRAMES_PER_SECOND = 30;

    public static JoyObject Create(string name, int hitPoints, Vector2Int worldPosition, Texture2D[] icons, string baseType, bool isAnimated, bool isWall = false)
    {
        JoyObject newObject = Instantiate(Resources.Load<JoyObject>("Prefabs/JoyObject"));

        newObject.JoyName = name;
        newObject.name = newObject.JoyName;

        newObject.m_HitPoints = hitPoints;

        newObject.m_WorldPosition = worldPosition;
        newObject.Move(newObject.m_WorldPosition);

        newObject.m_Icons = new Sprite[icons.Length];
        for (int i = 0; i < icons.Length; i++)
        {
            newObject.m_Icons[i] = Sprite.Create(icons[i], new Rect(0, 0, 16, 16), new Vector2(0, 0), 16);
        }

        newObject.BaseType = baseType;
        newObject.IsAnimated = isAnimated;
        newObject.IsWall = isWall;

        //If it's not animated, select a random icon to represent it
        if (!newObject.IsAnimated && icons != null)
        {
            newObject.ChosenIcon = RNG.Roll(0, icons.Length - 1);
        }
        else
        {
            newObject.ChosenIcon = 0;
        }

        newObject.GetComponent<SpriteRenderer>().sprite = newObject.Icon;

        newObject.m_LastIcon = 0;
        newObject.m_FramesSinceLastChange = 0;

        return newObject;

        //GUID = GUIDManager.AssignGUID();
    }

    public static JoyObject Create(string name, int hitPoints, Vector2Int position, List<Sprite> sprites, string baseType, bool isAnimated, bool isWall = false)
    {
        JoyObject newObject = Instantiate(Resources.Load<JoyObject>("Prefabs/Sprite"));

        newObject.JoyName = name;
        newObject.name = newObject.JoyName;

        newObject.m_HitPoints = hitPoints;

        newObject.m_WorldPosition = position;
        newObject.Move(newObject.m_WorldPosition);

        newObject.m_Icons = sprites.ToArray();

        newObject.BaseType = baseType;
        newObject.IsAnimated = isAnimated;
        newObject.IsWall = isWall;

        //If it's not animated, select a random icon to represent it
        if (!newObject.IsAnimated && sprites != null)
        {
            newObject.ChosenIcon = RNG.Roll(0, sprites.Count - 1);
        }
        else
        {
            newObject.ChosenIcon = 0;
        }

        newObject.GetComponent<SpriteRenderer>().sprite = newObject.Icon;

        newObject.m_LastIcon = 0;
        newObject.m_FramesSinceLastChange = 0;

        return newObject;
    }

    public void Initialise(string name, int hitPoints, Vector2Int position, List<Sprite> sprites, string baseType, bool isAnimated, bool isWall = false)
    {
        this.JoyName = name;
        this.name = this.JoyName;

        this.m_HitPoints = hitPoints;

        this.m_WorldPosition = position;
        this.Move(this.m_WorldPosition);

        this.m_Icons = sprites.ToArray();

        this.BaseType = baseType;
        this.IsAnimated = isAnimated;
        this.IsWall = isWall;

        //If it's not animated, select a random icon to represent it
        if (!this.IsAnimated && sprites != null)
        {
            this.ChosenIcon = RNG.Roll(0, sprites.Count - 1);
        }
        else
        {
            this.ChosenIcon = 0;
        }

        this.GetComponent<SpriteRenderer>().sprite = this.Icon;

        this.m_LastIcon = 0;
        this.m_FramesSinceLastChange = 0;
    }

    public void Move(Vector2Int newPosition)
    {
        m_WorldPosition = newPosition;
        this.transform.position = new Vector3(m_WorldPosition.x, m_WorldPosition.y);
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
	void Update ()
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

            this.GetComponent<SpriteRenderer>().sprite = Icon;

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

    public int GUID
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
}
