using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Physics;
using JoyLib.Code.Rollers;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class EntityFactory : IEntityFactory
    {
        protected INeedHandler NeedHandler { get; set; }

        protected IObjectIconHandler ObjectIcons { get; set; }

        protected ICultureHandler CultureHandler { get; set; }

        protected IEntitySexualityHandler SexualityHandler { get; set; }

        protected IEntityBioSexHandler BioSexHandler { get; set; }
        
        protected IGenderHandler GenderHandler { get; set; }
        
        protected IEntityRomanceHandler RomanceHandler { get; set; }

        protected IJobHandler JobHandler { get; set; }
        
        protected IPhysicsManager PhysicsManager { get; set; }
        
        protected IEntitySkillHandler SkillHandler { get; set; }
        
        protected RNG Roller { get; set; }

        public EntityFactory(
            INeedHandler needHandler,
            IObjectIconHandler objectIconHandler,
            ICultureHandler cultureHandler,
            IEntitySexualityHandler sexualityHandler,
            IEntityBioSexHandler sexHandler,
            IGenderHandler genderHandler,
            IEntityRomanceHandler romanceHandler,
            IJobHandler jobHandler,
            IPhysicsManager physicsManager,
            IEntitySkillHandler skillHandler,
            RNG roller)
        {
            Roller = roller;
            NeedHandler = needHandler;
            ObjectIcons = objectIconHandler;
            CultureHandler = cultureHandler;
            SexualityHandler = sexualityHandler;
            BioSexHandler = sexHandler;
            JobHandler = jobHandler;
            RomanceHandler = romanceHandler;
            GenderHandler = genderHandler;
            PhysicsManager = physicsManager;
            SkillHandler = skillHandler;
        }

        public IEntity CreateFromTemplate(
            IEntityTemplate template,
            Vector2Int position,
            IGrowingValue level = null,
            BasicValueContainer<IRollableValue> statistics = null,
            BasicValueContainer<IGrowingValue> skills = null,
            IEnumerable<IAbility> abilities = null,
            List<ICulture> cultures = null,
            IGender gender = null,
            IBioSex sex = null,
            ISexuality sexuality = null,
            IRomance romance = null,
            IJob job = null,
            Sprite[] sprites = null,
            IWorldInstance world = null,
            IDriver driver = null)
        {
            IJob selectedJob = job;
            IGender selectedGender = gender;
            IBioSex selectedSex = sex;
            ISexuality selectedSexuality = sexuality;
            IRomance selectedRomance = romance;
            Sprite[] selectedSprites = sprites;
            List<ICulture> creatureCultures = new List<ICulture>();
            IDriver selectedDriver = driver;
            BasicValueContainer<IRollableValue> selectedStatistics = statistics;
            BasicValueContainer<IGrowingValue> selectedSkills = skills;
            IEnumerable<IAbility> selectedAbilities = abilities;
            if (!(cultures is null))
            {
                creatureCultures.AddRange(cultures);
            }
            else
            {
                creatureCultures = new List<ICulture>();
                List<ICulture> cultureTypes = CultureHandler.GetByCreatureType(template.CreatureType);
                creatureCultures.AddRange(cultureTypes);
            }

            BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>();

            foreach (string need in template.Needs)
            {
                needs.Add(NeedHandler.GetRandomised(need));
            }

            int result = Roller.Roll(0, creatureCultures.Count);
            ICulture dominantCulture = creatureCultures[result];

            if (selectedStatistics is null)
            {
                selectedStatistics = dominantCulture.GetStats(template.Statistics);
            }

            if (selectedSkills is null)
            {
                selectedSkills = SkillHandler.GetDefaultSkillBlock(needs.Values);
                foreach (IGrowingValue skill in template.Skills.Values)
                {
                    selectedSkills.Add(skill);
                }
            }

            if (selectedAbilities is null)
            {
                selectedAbilities = new List<IAbility>();
            }

            if(selectedJob is null)
            {
                selectedJob = dominantCulture.ChooseJob(JobHandler.Jobs);
            }

            if(selectedSex is null)
            {
                selectedSex = dominantCulture.ChooseSex(BioSexHandler.Sexes);
            }

            if (selectedGender is null)
            {
                selectedGender = dominantCulture.ChooseGender(selectedSex, GenderHandler.Genders.ToArray());
            }

            if (selectedRomance is null)
            {
                selectedRomance = dominantCulture.ChooseRomance(RomanceHandler.Romances);
            }

            if(selectedSexuality is null)
            {
                selectedSexuality = dominantCulture.ChooseSexuality(SexualityHandler.Sexualities);
            }

            if(selectedSprites is null)
            {
                selectedSprites = ObjectIcons.GetSprites(dominantCulture.Tileset, template.JoyType);
            }

            if(selectedDriver is null)
            {
                selectedDriver = new StandardDriver(PhysicsManager);
            }

            IEntity entity = new Entity(
                template, 
                needs, 
                selectedStatistics, 
                selectedSkills, 
                selectedAbilities,
                creatureCultures, 
                selectedJob, 
                selectedGender,
                selectedSex, 
                selectedSexuality, 
                selectedRomance,
                position, 
                selectedSprites, 
                world,
                selectedDriver,
                new RNG());

            return entity;
        }

        public IEntity CreateLong(
            IEntityTemplate template,
            BasicValueContainer<IRollableValue> statistics,
            BasicValueContainer<INeed> needs,
            BasicValueContainer<IGrowingValue> skills,
            IEnumerable<IAbility> abilities,
            IGrowingValue level,
            float experience,
            IJob job,
            IGender gender,
            IBioSex sex,
            ISexuality sexuality,
            IRomance romance,
            Vector2Int position,
            Sprite[] sprites,
            IItemInstance naturalWeapons,
            NonUniqueDictionary<string, IItemInstance> equipment,
            List<IItemInstance> backpack,
            List<string> identifiedItems,
            IEnumerable<IJob> jobs,
            IWorldInstance world,
            List<ICulture> cultures = null,
            IDriver driver = null)
        {
            List<ICulture> creatureCultures = new List<ICulture>();
            if (cultures != null)
            {
                creatureCultures.AddRange(cultures);
            }
            else
            {
                List<ICulture> cultureTypes = CultureHandler.GetByCreatureType(template.CreatureType);
                creatureCultures.AddRange(cultureTypes);
            }

            IDriver selectedDriver = driver;
            if(selectedDriver is null)
            {
                selectedDriver = new StandardDriver(PhysicsManager);
            }

            IEntity entity = new Entity(
                template,
                statistics, 
                needs, 
                skills, 
                abilities, 
                creatureCultures, 
                job, 
                gender,
                sex, 
                sexuality, 
                romance,
                position,
                sprites, 
                naturalWeapons, 
                equipment, 
                backpack, 
                identifiedItems, 
                jobs, 
                world,
                selectedDriver,
                new RNG());

            return entity;
        }
    }
}