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
using JoyLib.Code.World;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    protected StateManager m_StateManager;

    protected ObjectIconHandler m_ObjectIcons;

    protected CultureHandler m_CultureHandler;

    protected EntityTemplateHandler m_EntityTemplateHandler;

	// Use this for initialization
	void Start ()
    {
        m_ObjectIcons = this.GetComponent<ObjectIconHandler>();
        m_CultureHandler = this.GetComponent<CultureHandler>();
        m_EntityTemplateHandler = this.GetComponent<EntityTemplateHandler>();

        InitialiseEverything();

        m_StateManager = new StateManager();

        //REPLACE THIS WITH AN ACTUAL ENTITY CONSTRUCTOR
        BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>();
        NeedHandler needHandler = this.GetComponent<NeedHandler>();
        INeed testingNeed = needHandler.GetRandomised("thirst");
        needs.Add(testingNeed);

        List<CultureType> cultures = m_CultureHandler.GetByCreatureType("Human");
        CultureType culture = cultures[0];
        EntityTemplate human = m_EntityTemplateHandler.Get("human");
        JobType jobType = culture.ChooseJob();

        IGrowingValue level = new ConcreteGrowingValue("level", 1, 100, 0, GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                                                        new StandardRoller(), new NonUniqueDictionary<INeed, float>());

        EntityFactory entityFactory = new EntityFactory();
        Entity player = entityFactory.Create(
            human, 
            needs, 
            level, 
            jobType, 
            culture.ChooseSex(), 
            culture.ChooseSexuality(), 
            Vector2Int.zero, 
            m_ObjectIcons.GetSprites(human.Tileset, jobType.Name), 
            null, 
            new List<CultureType>() { culture });
        player.PlayerControlled = true;

        m_StateManager.ChangeState(new WorldCreationState(player));

        //GameObject.Find("NeedsText").GetComponent<GUINeedsAlert>().SetPlayer(player);
    }

    private void InitialiseEverything()
    {
        RNG.instance.SetSeed(DateTime.Now.Millisecond);
        AbilityHandler.instance.Initialise();
        EntityBioSexHandler.instance.Load(m_CultureHandler.Cultures);
        EntitySexualityHandler.Load(m_CultureHandler.Cultures);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(!(m_StateManager is null))
        {
            m_StateManager.Update();
        }

	}
}
