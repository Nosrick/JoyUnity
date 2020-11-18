namespace JoyLib.Code.Cultures
{
    public struct NameData
    {
        public string name;
        public int[] chain;
        public string[] genders;

        public NameData(string nameRef, int[] chainRef, string[] gendersRef)
        {
            name = nameRef;
            chain = chainRef;
            genders = gendersRef;
        }
    }
}