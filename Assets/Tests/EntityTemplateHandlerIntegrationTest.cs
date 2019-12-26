using JoyLib.Code.Entities;
using NUnit.Framework;

namespace JoyTest
{
    public class EntityTemplateHandlerIntegrationTest
    {
        [Test]
        public void LoadTypes_ShouldHaveValidData()
        {
            //given
            EntityTemplateHandler.Initialise();

            //when
            EntityTemplate[] entityTemplates = EntityTemplateHandler.Templates;

            //then
            foreach(EntityTemplate template in entityTemplates)
            {
                Assert.That(template.Statistics.Collection, Is.Not.Empty);
                Assert.That(template.Skills.Collection, Is.Not.Empty);
                Assert.That(template.Slots, Is.Not.Empty);
                Assert.That(template.Tags, Is.Not.Empty);
                Assert.False(template.JoyType == "DEFAULT");
                Assert.False(template.CreatureType == "DEFAULT");
                Assert.False(template.Tileset == "DEFAULT");
            }
        }
    }
}
