namespace JoyLib.Code.Cultures
{
    public struct NameData
    {
        public string name;
        public int[] chain;
        public string[] sexes;

        public NameData(string nameRef, int[] chainRef, string[] sexesRef)
        {
            name = nameRef;
            chain = chainRef;
            sexes = sexesRef;
        }
    }
}