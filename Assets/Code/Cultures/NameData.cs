using JoyLib.Code.Entities;

namespace JoyLib.Code.Cultures
{
    public struct NameData
    {
        public string name;
        public bool isSurname;
        public Sex sex;

        public NameData(string nameRef, bool isSurnameRef, Sex sexRef)
        {
            name = nameRef;
            isSurname = isSurnameRef;
            sex = sexRef;
        }
    }
}