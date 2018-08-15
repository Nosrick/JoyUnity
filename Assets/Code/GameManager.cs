using Joy.Code.Managers;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Scripting;
using JoyLib.Code.States;
using JoyLib.Code.Unity.GUI;
using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private StateManager m_StateManager;

	// Use this for initialization
	void Start ()
    {
        InitialiseEverything();

        m_StateManager = new StateManager();

        Entity thief = new Entity(EntityTemplateHandler.Get("Human"), EntityNeed.GetFullRandomisedNeeds(), 1, JobHandler.Get("Thief"), Sex.Neutral, Sexuality.Bisexual, Vector2Int.zero, ObjectIcons.GetSprites("Jobs", "Thief").ToList(), null)
        {
            PlayerControlled = true
        };

        m_StateManager.ChangeState(new WorldCreationState(thief));

        GameObject.Find("NeedsText").GetComponent<GUINeedsAlert>().SetPlayer(thief);
    }

    private void InitialiseEverything()
    {
        RNG.SetSeed(DateTime.Now.Millisecond);
        ScriptingEngine.Initialise();
        ObjectIcons.Load();
        NeedHandler.Initialise();
        AbilityHandler.Initialise();
        JobHandler.Initialise();
        CultureHandler.Initialise();
        MaterialHandler.Initialise();
        ItemHandler.LoadItems();
        NameProvider.Initialise();
        EntityTemplateHandler.Initialise();
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_StateManager.Update();
	}
}
