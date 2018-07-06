namespace JoyLib.Code.Entities.Items
{
    [System.Serializable()]
    public class ItemMaterial
    {
        public ItemMaterial()
        {
            name = "DEFAULT MATERIAL";
            hardness = 1.0f;
            bonus = 0;
            weight = 1.0f;
        }

        public ItemMaterial(string nameRef, float hardnessRef, int bonusRef, float weightRef)
        {
            name = nameRef;
            hardness = hardnessRef;
            bonus = bonusRef;
            weight = weightRef;
        }

        //Hardness will be multiplied by the item's size modifier to find its hit points

        //The bonus the material applies to any checks made with it

        //How many grams per cm^3

        public string name
        {
            get;
            set;
        }

        public float hardness
        {
            get;
            set;
        }

        public int bonus
        {
            get;
            set;

        }

        public float weight
        {
            get;
            set;
        }
    }
}