using JoyLib.Code.Entities;
using Xunit;

namespace JoyTest
{
    public class EntityTemplateHandlerIntegrationTest
    {
        [Fact]
        public void LoadTypes_ShouldHaveValidData()
        {
            //given
            EntityTemplateHandler.Initialise();

            //when
            EntityTemplate[] entityTemplates = EntityTemplateHandler.Templates;

            //then
            foreach(EntityTemplate template in entityTemplates)
            {
                Assert.NotEmpty(template.Statistics.Collection);
                Assert.NotEmpty(template.Skills.Collection);
                Assert.NotEmpty(template.Slots);
                Assert.NotEmpty(template.Tags);
                Assert.False(template.JoyType == "DEFAULT");
                Assert.False(template.CreatureType == "DEFAULT");
                Assert.False(template.Tileset == "DEFAULT");
            }
        }
    }
}
