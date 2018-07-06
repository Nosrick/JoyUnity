using JoyLib.Code.Entities;

namespace JoyLib.Code.Cultures
{
    public struct NameData
    {
        public string name;
        public bool isSurname;
        public Gender gender;

        public NameData(string nameRef, bool isSurnameRef, Gender genderRef)
        {
            name = nameRef;
            isSurname = isSurnameRef;
            gender = genderRef;
        }
    }
}