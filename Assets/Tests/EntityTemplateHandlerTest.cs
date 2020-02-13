using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using NUnit.Framework;

namespace JoyTest
{
    public class EntityTemplateHandlerTest
    {
        private EntityTemplateHandler target;

        private NeedHandler needHandler;

        [SetUp]
        public void SetUp()
        {
            target = new EntityTemplateHandler();
            needHandler = new NeedHandler();
        }

        [Test]
        public void LoadTypes_ShouldHaveValidData()
        {
            //given
            EntityTemplate[] entityTemplates = EntityTemplateHandler.instance.Templates;

            //when

            //then
            Assert.That(entityTemplates, Is.Not.Empty);
            foreach(EntityTemplate template in entityTemplates)
            {
                Assert.That(template.Statistics.Collection, Is.Not.Empty);
                Assert.That(template.Slots, Is.Not.Empty);
                Assert.That(template.Tags, Is.Not.Empty);
                Assert.False(template.JoyType == "DEFAULT");
                Assert.False(template.CreatureType == "DEFAULT");
                Assert.False(template.Tileset == "DEFAULT");
            }
        }
    }
}
