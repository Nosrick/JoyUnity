using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace JoyLib.Code.Cultures
{
    public class CultureHandler : ICultureHandler
    {
        protected Dictionary<string, ICulture> m_Cultures;

        public IEnumerable<ICulture> Values => this.m_Cultures.Values;

        public CultureHandler()
        {
            this.Initialise();
        }

        protected void Initialise()
        {
            if (this.m_Cultures is null)
            {
                this.m_Cultures = this.Load().ToDictionary(x => x.CultureName, x => x);
            }
        }

        public ICulture Get(string name)
        {
            return this.GetByCultureName(name);
        }

        public IEnumerable<ICulture> Load()
        {
            string folderPath = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Cultures";
            string[] files = Directory.GetFiles(folderPath, "*.json");

            IObjectIconHandler objectIcons = GlobalConstants.GameManager.ObjectIconHandler;

            List<ICulture> cultures = new List<ICulture>();

            foreach (string file in files)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        try
                        {
                            JObject jToken = JObject.Load(jsonReader);

                            if (jToken.IsNullOrEmpty())
                            {
                                continue;
                            }

                            foreach (JToken child in jToken["Cultures"])
                            {
                                string cultureName = (string) child["CultureName"];
                                int nonConformingGenderChance = (int) child["NonConformingGenderChance"];
                                IEnumerable<string> rulers = child["Rulers"].Select(token => (string) token);
                                IEnumerable<string> crimes = child["Crimes"].Select(token => (string) token);
                                IEnumerable<string> inhabitants =
                                    child["Inhabitants"].Select(token => (string) token);
                                IEnumerable<string> relationships =
                                    child["Relationships"].Select(token => (string) token);

                                JToken dataArray = child["Names"];
                                List<NameData> nameData = new List<NameData>();
                                foreach (var data in dataArray)
                                {
                                    string name = (string) data["Name"];
                                    int[] chain = data["Chain"]?.Select(token => (int) token).ToArray();
                                    if (chain.IsNullOrEmpty())
                                    {
                                        chain = new[] {0};
                                    }

                                    string[] genderNames = data["Gender"]?.Select(token => (string) token).ToArray();
                                    if (genderNames.IsNullOrEmpty())
                                    {
                                        genderNames = new[] {"all"};
                                    }

                                    int[] groups = data["Group"]?.Select(token => (int) token).ToArray();
                                    if (groups.IsNullOrEmpty())
                                    {
                                        groups = new int[0];
                                    }
                                    nameData.Add(new NameData(
                                        name,
                                        chain,
                                        genderNames,
                                        groups));
                                }

                                dataArray = child["Sexualities"];
                                IDictionary<string, int> sexualities = dataArray.Select(token =>
                                        new KeyValuePair<string, int>(
                                            (string) token["Name"],
                                            (int) token["Chance"]))
                                    .ToDictionary(x => x.Key, x => x.Value);

                                dataArray = child["Romances"];
                                IDictionary<string, int> romances = dataArray.Select(token =>
                                        new KeyValuePair<string, int>(
                                            (string) token["Name"],
                                            (int) token["Chance"]))
                                    .ToDictionary(x => x.Key, x => x.Value);

                                dataArray = child["Genders"];
                                IDictionary<string, int> genders = dataArray.Select(token =>
                                        new KeyValuePair<string, int>(
                                            (string) token["Name"],
                                            (int) token["Chance"]))
                                    .ToDictionary(x => x.Key, x => x.Value);

                                dataArray = child["Sexes"];
                                IDictionary<string, int> sexes = dataArray.Select(token =>
                                        new KeyValuePair<string, int>(
                                            (string) token["Name"],
                                            (int) token["Chance"]))
                                    .ToDictionary(x => x.Key, x => x.Value);

                                dataArray = child["Statistics"];
                                IDictionary<string, Tuple<int, int>> statistics = dataArray.Select(token =>
                                        new KeyValuePair<string, Tuple<int, int>>(
                                            (string) token["Name"],
                                            new Tuple<int, int>(
                                                (int) token["Chance"],
                                                (int) token["Magnitude"])))
                                    .ToDictionary(x => x.Key, x => x.Value);

                                dataArray = child["Jobs"];
                                IDictionary<string, int> jobPrevalence = dataArray.Select(token =>
                                        new KeyValuePair<string, int>(
                                            (string) token["Name"],
                                            (int) token["Chance"]))
                                    .ToDictionary(x => x.Key, x => x.Value);

                                dataArray = child["TileSet"];
                                string tileSetName = (string) dataArray["Name"];

                                objectIcons.AddSpriteDataFromJson(tileSetName, dataArray["SpriteData"]);

                                dataArray = child["UIColours"];

                                IDictionary<string, IDictionary<string, Color>> cursorColours =
                                    new Dictionary<string, IDictionary<string, Color>>();
                                try
                                {
                                    cursorColours = this.ExtractColourData(dataArray, "CursorColours");
                                }
                                catch (Exception e)
                                {
                                    GlobalConstants.ActionLog.AddText(
                                        "Could not find cursor colours in file " + file,
                                        LogLevel.Error);
                                    GlobalConstants.ActionLog.StackTrace(e);
                                    cursorColours.Add(
                                        "DefaultCursor",
                                        new Dictionary<string, Color>
                                        {
                                            {"default", Color.magenta}
                                        });
                                }

                                IDictionary<string, IDictionary<string, Color>> backgroundColours =
                                    new Dictionary<string, IDictionary<string, Color>>();
                                try
                                {
                                    backgroundColours = this.ExtractColourData(dataArray, "BackgroundColours");
                                }
                                catch (Exception e)
                                {
                                    GlobalConstants.ActionLog.AddText(
                                        "Could not find background colours in file " + file,
                                        LogLevel.Warning);
                                    GlobalConstants.ActionLog.StackTrace(e);
                                    backgroundColours.Add(
                                        "DefaultWindow",
                                        new Dictionary<string, Color>
                                        {
                                            {"default", Color.magenta}
                                        });
                                }

                                IDictionary<string, Color> mainFontColours = new Dictionary<string, Color>();
                                try
                                {
                                    var fontColours = dataArray["FontColours"];
                                    foreach (var colour in fontColours)
                                    {
                                        mainFontColours.Add(
                                            (string) colour["Name"],
                                            GraphicsHelper.ParseHTMLString((string) colour["Value"]));
                                    }
                                }
                                catch (Exception e)
                                {
                                    GlobalConstants.ActionLog.AddText(
                                        "Could not find main font colour in file " + file,
                                        LogLevel.Warning);
                                    GlobalConstants.ActionLog.StackTrace(e);
                                    mainFontColours.Add(
                                        "Font",
                                        Color.black);
                                }

                                cultures.Add(
                                    new CultureType(
                                        cultureName,
                                        tileSetName,
                                        rulers,
                                        crimes,
                                        nameData,
                                        jobPrevalence,
                                        inhabitants,
                                        sexualities,
                                        sexes,
                                        statistics,
                                        relationships,
                                        romances,
                                        genders,
                                        nonConformingGenderChance,
                                        backgroundColours,
                                        cursorColours,
                                        mainFontColours));
                            }
                        }
                        catch (Exception e)
                        {
                            GlobalConstants.ActionLog.AddText("Could not load cultures from file: " + file,
                                LogLevel.Error);
                            GlobalConstants.ActionLog.StackTrace(e);
                        }
                        finally
                        {
                            jsonReader.Close();
                            reader.Close();
                        }
                    }
                }
            }

            return cultures;
        }

        protected IDictionary<string, IDictionary<string, Color>> ExtractColourData(
            JToken element,
            string elementName)
        {
            IDictionary<string, IDictionary<string, Color>> colours =
                new Dictionary<string, IDictionary<string, Color>>();
            foreach(var colour in element[elementName])
            {
                string name = (string) colour["Name"];
                foreach (var data in colour["Colour"])
                {
                    string partName = (string) data["Name"];
                    Color c = GraphicsHelper.ParseHTMLString((string) data["Value"]);

                    if (colours.ContainsKey(name))
                    {
                        colours[name].Add(partName, c);
                    }
                    else
                    {
                        colours.Add(name, new Dictionary<string, Color>
                        {
                            {partName, c}
                        });
                    }
                }
            }
            return colours;
        }

        public ICulture GetByCultureName(string name)
        {
            this.Initialise();

            if (this.m_Cultures.ContainsKey(name))
            {
                return this.m_Cultures[name];
            }

            return null;
        }

        public List<ICulture> GetByCreatureType(string type)
        {
            this.Initialise();

            try
            {
                Dictionary<string, ICulture> cultures = this.m_Cultures.Where(
                        culture => culture.Value.Inhabitants.Contains(
                            type, GlobalConstants.STRING_COMPARER))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
                return cultures.Values.ToList();
            }
            catch (Exception e)
            {
                GlobalConstants.ActionLog.AddText("Could not find a culture for creature type " + type);
                throw new InvalidOperationException("Could not find a culture for creature type " + type, e);
            }
        }

        public IEnumerable<ICulture> Cultures
        {
            get
            {
                this.Initialise();

                return this.m_Cultures.Values.ToArray();
            }
        }

        public void Dispose()
        {
            string[] keys = this.m_Cultures.Keys.ToArray();
            foreach (string key in keys)
            {
                this.m_Cultures[key] = null;
            }

            this.m_Cultures = null;
        }
    }
}