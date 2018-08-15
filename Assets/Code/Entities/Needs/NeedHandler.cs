using JoyLib.Code.Helpers;
using System.Collections.Generic;
using System.IO;

namespace JoyLib.Code.Entities.Needs
{
    public static class NeedHandler
    {
        private static Dictionary<string, string> s_InteractionFileContents;

        public static void Initialise()
        {
            if (s_InteractionFileContents != null)
            {
                return;
            }

            s_InteractionFileContents = new Dictionary<string, string>();

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.SCRIPTS_FOLDER + "Needs", "*.lua");
            foreach(string file in files)
            {
                string name = FileNameExtractor.ExtractName(file, 3);
                string contents = File.ReadAllText(file);
                s_InteractionFileContents.Add(name, contents);
            }
        }

        public static string Get(string name)
        {
            if(s_InteractionFileContents.ContainsKey(name))
            {
                return s_InteractionFileContents[name];
            }
            return "";
        }
    }
}
