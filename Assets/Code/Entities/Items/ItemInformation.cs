using JoyLib.Code.Entities.Abilities;

namespace JoyLib.Code.Entities.Items
{
    public struct IdentifiedItem
    {
        public string name;
        public string description;
        public int value;
        public IAbility[] abilities;
        public int weighting;
        public int lightLevel;
        public string skill;
        public string[] materials;
        public string[] tags;
        public float size;
        public string[] slots;
        public string spriteSheet;

        public IdentifiedItem(string nameRef, string[] tagsRef, string descriptionRef, int valueRef, IAbility[] abilitiesRef, int weightingRef,
            string skillRef, string[] materialsRef, float sizeRef, string[] slotsRef, string spriteSheetRef, int lightLevelRef = 0)
        {
            this.name = nameRef;
            this.tags = tagsRef;
            this.description = descriptionRef;
            this.value = valueRef;
            this.abilities = abilitiesRef;
            this.weighting = weightingRef;
            this.skill = skillRef;
            this.materials = materialsRef;
            this.size = sizeRef;
            this.slots = slotsRef;
            this.spriteSheet = spriteSheetRef;
            this.lightLevel = lightLevelRef;
        }
    }

    public struct UnidentifiedItem
    {
        public string name;
        public string description;

        public UnidentifiedItem(string nameRef, string descriptionRef)
        {
            this.name = nameRef;
            this.description = descriptionRef;
        }
    }
}