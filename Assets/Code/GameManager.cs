﻿using Joy.Code.Managers;
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
using System.Collections.Generic;
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

        //REPLACE THIS WITH AN ACTUAL ENTITY CONSTRUCTOR
        Dictionary<string, INeed> needs = new Dictionary<string, INeed>();
        INeed hunger = NeedHandler.GetRandomised("Hunger");
        needs.Add(hunger.GetName(), hunger);

        Entity thief = WorldState.EntityHandler.Create(EntityTemplateHandler.Get("Human"), needs, 1, JobHandler.Get("Thief"), Sex.Neutral, Sexuality.Bisexual, Vector2Int.zero, ObjectIcons.GetSprites("Jobs", "Thief"), null);
        thief.PlayerControlled = true;

        m_StateManager.ChangeState(new WorldCreationState(thief));

        GameObject.Find("NeedsText").GetComponent<GUINeedsAlert>().SetPlayer(thief);
    }

    private void InitialiseEverything()
    {
        RNG.SetSeed(DateTime.Now.Millisecond);
        if(ScriptingEngine.Initialise() == false)
        {
            Debug.Log("COULD NOT INITIALISE SCRIPTING ENGINE.");
            Destroy(this);
            return;
        }
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
