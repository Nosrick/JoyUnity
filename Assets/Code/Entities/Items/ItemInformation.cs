namespace JoyLib.Code.Entities.Items
{
    public struct IdentifiedItem
    {
        public string name;
        public string category;
        public string description;
        public int value;
        public string interactionFile;
        public int weighting;
        public string skill;
        public string[] materials;
        public float size;
        public string slot;

        public IdentifiedItem(string nameRef, string categoryRef, string descriptionRef, int valueRef, string fileRef, int weightingRef,
            string skillRef, string[] materialsRef, float sizeRef, string slotRef)
        {
            name = nameRef;
            category = categoryRef;
            description = descriptionRef;
            value = valueRef;
            interactionFile = fileRef;
            weighting = weightingRef;
            skill = skillRef;
            materials = materialsRef;
            size = sizeRef;
            slot = slotRef;
        }
    }

    public struct UnidentifiedItem
    {
        public string name;
        public string description;

        public UnidentifiedItem(string nameRef, string descriptionRef)
        {
            name = nameRef;
            description = descriptionRef;
        }
    }
}