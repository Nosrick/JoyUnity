using Joy.Code.Managers;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
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
        needs.Add(hunger.Name, hunger);

        List<CultureType> cultures = CultureHandler.GetByCreatureType("Human");
        CultureType culture = cultures[0];

        Entity thief = WorldState.EntityHandler.Create(EntityTemplateHandler.Get("Human"), needs, 1, JobHandler.Get("Thief"), culture.ChooseSex(), culture.ChooseSexuality(), Vector2Int.zero, ObjectIcons.GetSprites("Jobs", "Thief"), null, new List<CultureType>() { culture });
        thief.PlayerControlled = true;

        m_StateManager.ChangeState(new WorldCreationState(thief));

        GameObject.Find("NeedsText").GetComponent<GUINeedsAlert>().SetPlayer(thief);
    }

    private void InitialiseEverything()
    {
        RNG.SetSeed(DateTime.Now.Millisecond);
        ActionLog.OpenLog();

        EntityStatistic.LoadStatistics();

        if (ScriptingEngine.Initialise() == false)
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
        EntityBioSexHandler.Load(CultureHandler.Cultures);
        EntitySexualityHandler.Load(CultureHandler.Cultures);
        MaterialHandler.Initialise();
        ItemHandler.LoadItems();
        EntityTemplateHandler.Initialise();
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_StateManager.Update();
	}
}
