using Joy.Code.Managers;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.States;
using JoyLib.Code.Unity.GUI;
using System;
using System.Collections.Generic;
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
        BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>();
        INeed testingNeed = NeedHandler.GetRandomised("thirst");
        needs.Add(testingNeed);

        List<CultureType> cultures = CultureHandler.GetByCreatureType("Human");
        CultureType culture = cultures[0];
        EntityTemplate human = EntityTemplateHandler.Get("human");
        JobType jobType = culture.ChooseJob();

        IGrowingValue level = new ConcreteGrowingValue("level", 1, 100, 0, GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                                                        new StandardRoller(), new NonUniqueDictionary<INeed, float>());

        Entity player = WorldState.EntityHandler.Create(
            human, 
            needs, 
            level, 
            jobType, 
            culture.ChooseSex(), 
            culture.ChooseSexuality(), 
            Vector2Int.zero, 
            ObjectIconHandler.GetSprites(human.Tileset, jobType.Name), 
            null, 
            new List<CultureType>() { culture });
        player.PlayerControlled = true;

        m_StateManager.ChangeState(new WorldCreationState(player));

        GameObject.Find("NeedsText").GetComponent<GUINeedsAlert>().SetPlayer(player);
    }

    private void InitialiseEverything()
    {
        RNG.SetSeed(DateTime.Now.Millisecond);
        ActionLog.OpenLog();

        if (ScriptingEngine.Initialise() == false)
        {
            Debug.Log("COULD NOT INITIALISE SCRIPTING ENGINE.");
            Destroy(this);
            return;
        }
        ObjectIconHandler.Load();
        NeedHandler.Initialise();
        AbilityHandler.Initialise();
        JobHandler.Initialise();
        CultureHandler.Initialise();
        EntityBioSexHandler.Load(CultureHandler.Cultures);
        EntitySexualityHandler.Load(CultureHandler.Cultures);
        MaterialHandler.Initialise();
        EntityTemplateHandler.Initialise();
        LiveItemHandler.Initialise();
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_StateManager.Update();
	}
}
