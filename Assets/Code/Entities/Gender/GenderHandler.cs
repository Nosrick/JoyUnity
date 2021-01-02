using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Helpers;

namespace JoyLib.Code.Entities.Gender
{
    public class GenderHandler : IGenderHandler
    {
        public HashSet<IGender> Genders { get; protected set; }

        public GenderHandler()
        {
            this.Initialise();
        }

        protected void Initialise()
        {
            if (this.Genders is null)
            {
                this.Genders = this.LoadGenders();
            }
        }

        protected HashSet<IGender> LoadGenders()
        {
            HashSet<IGender> genders = new HashSet<IGender>();
            try
            {
                string[] files =
                    Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "/Genders",
                        "*.xml", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    XElement doc = XElement.Load(file);

                    genders = new HashSet<IGender>(from gender in doc.Elements("Gender")
                        select new BaseGender(
                            gender.Element("Name").GetAs<string>(),
                            gender.Element("Possessive").GetAs<string>(),
                            gender.Element("PersonalSubject").GetAs<string>(),
                            gender.Element("PersonalObject").GetAs<string>(),
                            gender.Element("Reflexive").GetAs<string>(),
                            gender.Element("PossessivePlural").GetAs<string>(),
                            gender.Element("ReflexivePlural").GetAs<string>(),
                            gender.Element("IsOrAre").DefaultIfEmpty("is")));
                }

                return genders;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return genders;
            }
        }

        public IGender Get(string name)
        {
            if (this.Genders is null)
            {
                this.Initialise();
            }

            return this.Genders.First(gender => gender.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}