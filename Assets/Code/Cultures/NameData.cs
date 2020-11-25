namespace JoyLib.Code.Cultures
{
    public struct NameData
    {
        public string name;
        public int[] chain;
        public string[] genders;
        public int[] groups;

        public NameData(string nameRef, int[] chainRef, string[] gendersRef, int[] groupsRef)
        {
            name = nameRef;
            chain = chainRef;
            genders = gendersRef;
            groups = groupsRef;
        }
    }
}