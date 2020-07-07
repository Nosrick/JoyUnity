using Joy.Code.Managers;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using JoyLib.Code.States;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    protected StateManager m_StateManager;

	// Use this for initialization
	void Start ()
    {
        ObjectIconHandler objectIcons = this.GetComponent<ObjectIconHandler>();
        CultureHandler cultureHandler = this.GetComponent<CultureHandler>();
        EntityTemplateHandler entityTemplateHandler = this.GetComponent<EntityTemplateHandler>();
        EntityBioSexHandler bioSexHandler = this.GetComponent<EntityBioSexHandler>();
        EntitySexualityHandler sexualityHandler = this.GetComponent<EntitySexualityHandler>();
        JobHandler jobHandler = this.GetComponent<JobHandler>();

        InitialiseEverything();

        m_StateManager = new StateManager();

        //REPLACE THIS WITH AN ACTUAL ENTITY CONSTRUCTOR
        BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>();
        NeedHandler needHandler = this.GetComponent<NeedHandler>();
        INeed testingNeed = needHandler.GetRandomised("thirst");
        needs.Add(testingNeed);

        List<CultureType> cultures = cultureHandler.GetByCreatureType("Human");
        CultureType culture = cultures[0];
        EntityTemplate human = entityTemplateHandler.Get("human");
        JobType jobType = culture.ChooseJob(jobHandler.Jobs);

        IGrowingValue level = new ConcreteGrowingValue("level", 1, 100, 0, GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                                                        new StandardRoller(), new NonUniqueDictionary<INeed, float>());

        EntityFactory entityFactory = new EntityFactory();
        Entity player = entityFactory.CreateFromTemplate(
            human, 
            level, 
            Vector2Int.zero, 
            new List<CultureType>() { culture },
            culture.ChooseSex(bioSexHandler.Sexes), 
            culture.ChooseSexuality(sexualityHandler.Sexualities), 
            jobType, 
            objectIcons.GetSprites(human.Tileset, jobType.Name));
        player.PlayerControlled = true;

        m_StateManager.ChangeState(new WorldCreationState(player));

        //GameObject.Find("NeedsText").GetComponent<GUINeedsAlert>().SetPlayer(player);
    }

    private void InitialiseEverything()
    {
        RNG.instance.SetSeed(DateTime.Now.Millisecond);
        AbilityHandler.instance.Initialise();
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
