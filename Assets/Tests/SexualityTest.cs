using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Rollers;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Sexes;
using NUnit.Framework;
using UnityEngine;

namespace JoyTest
{
    public class SexualityTest
    {
        private Entity heteroMaleHuman;
        private Entity heteroFemaleHuman;

        private Entity homoMaleHumanLeft;
        private Entity homoMaleHumanRight;

        private Entity homoFemaleHumanLeft;
        private Entity homoFemaleHumanRight;

        private Entity biRandomHuman;

        [SetUp]
        public void SetUpHumans()
        {
            CultureHandler.instance.Initialise();
            CultureType[] cultures = CultureHandler.instance.Cultures;
            EntityBioSexHandler.instance.Load(cultures);
            EntitySexualityHandler.Load(cultures);

            IBioSex female = EntityBioSexHandler.instance.Get("female");
            IBioSex male = EntityBioSexHandler.instance.Get("male");

            ISexuality heterosexual = EntitySexualityHandler.Get("heterosexual");
            ISexuality homosexual = EntitySexualityHandler.Get("homosexual");
            ISexuality bisexual = EntitySexualityHandler.Get("bisexual");

            BasicValueContainer<INeed> emptyContainer = new BasicValueContainer<INeed>();

            ConcreteGrowingValue level = new ConcreteGrowingValue(
                    "level",
                    1,
                    0,
                    0,
                    7,
                    new StandardRoller(),
                    new NonUniqueDictionary<INeed, float>());

            EntityTemplateHandler.instance.Initialise();
            EntityTemplate humanTemplate = EntityTemplateHandler.instance.Get("human");

            heteroFemaleHuman = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer,
                level,
                JobHandler.GetRandom(),
                female,
                heterosexual,
                Vector2Int.zero,
                null,
                null,
                null);

            heteroMaleHuman = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer,
                level,
                JobHandler.GetRandom(),
                male,
                heterosexual,
                Vector2Int.zero,
                null,
                null,
                null);

            homoMaleHumanLeft = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer,
                level,
                JobHandler.GetRandom(),
                male,
                homosexual,
                Vector2Int.zero,
                null,
                null,
                null);

            homoMaleHumanRight = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer,
                level,
                JobHandler.GetRandom(),
                male,
                homosexual,
                Vector2Int.zero,
                null,
                null,
                null);
        }

        [Test]
        public void Heterosexual_WillMateWith_AcceptsHeteroPartners()
        {

        }
    }
}