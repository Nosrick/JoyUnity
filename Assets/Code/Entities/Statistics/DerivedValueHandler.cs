using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Helpers;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Entities.Statistics
{
    public class DerivedValueHandler : IDerivedValueHandler
    {
        protected Dictionary<string, string> Formulas { get; set; }
        protected Dictionary<string, string> EntityStandardFormulas { get; set; }
        protected Dictionary<string, string> ItemStandardFormulas { get; set; }
        protected Dictionary<string, Color> DerivedValueBackgroundColours { get; set; }
        protected Dictionary<string, Color> DerivedValueTextColours { get; set; }
        protected Dictionary<string, Color> DerivedValueOutlineColours { get; set; }
        
        protected static IEntityStatisticHandler StatisticHandler { get; set; }
        protected static IEntitySkillHandler SkillHandler { get; set; }

        protected readonly string ENTITY_FILE;
        protected readonly string ITEM_FILE;

        public DerivedValueHandler(IEntityStatisticHandler statisticHandler = null,
            IEntitySkillHandler skillHandler = null)
        {
            if (StatisticHandler is null)
            {
                StatisticHandler = statisticHandler;
            }

            if (SkillHandler is null)
            {
                SkillHandler = skillHandler;
            }

            this.ENTITY_FILE = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER +
                               "EntityDerivedValues.xml";

            this.ITEM_FILE = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "ItemDerivedValues.xml";

            this.DerivedValueOutlineColours = new Dictionary<string, Color>();
            this.DerivedValueBackgroundColours = new Dictionary<string, Color>();
            this.DerivedValueTextColours = new Dictionary<string, Color>();
            this.EntityStandardFormulas = this.LoadFormulasFromFile(this.ENTITY_FILE);
            this.ItemStandardFormulas = this.LoadFormulasFromFile(this.ITEM_FILE);
            this.Formulas = new Dictionary<string, string>(this.EntityStandardFormulas);
            foreach (KeyValuePair<string, string> pair in this.ItemStandardFormulas)
            {
                this.Formulas.Add(pair.Key, pair.Value);
            }
        }

        public Dictionary<string, string> LoadFormulasFromFile(string file)
        {
            Dictionary<string, string> formulas = new Dictionary<string, string>();
            
            XElement doc = XElement.Load(file);
            foreach (XElement dv in doc.Elements("DerivedValue"))
            {
                try
                {
                    string name = dv.Element("Name").GetAs<string>().ToLower();
                    string colourCode = dv.Element("BackgroundColour").DefaultIfEmpty("#888888FF");
                    Color colour = new Color();
                    ColorUtility.TryParseHtmlString(colourCode, out colour);
                    formulas.Add(
                        name,
                        dv.Element("Formula").GetAs<string>().ToLower()); 
                    this.DerivedValueBackgroundColours.Add(name, colour);

                    colourCode = dv.Element("TextColour").DefaultIfEmpty("#FFFFFFFF");
                    ColorUtility.TryParseHtmlString(colourCode, out colour);
                    this.DerivedValueTextColours.Add(name, colour);

                    colourCode = dv.Element("OutlineColour").DefaultIfEmpty("#000000FF");
                    ColorUtility.TryParseHtmlString(colourCode, out colour);
                    this.DerivedValueOutlineColours.Add(name, colour);
                }
                catch (Exception e)
                {
                    GlobalConstants.ActionLog.AddText(e.Message, LogLevel.Error);
                    GlobalConstants.ActionLog.AddText(e.StackTrace, LogLevel.Error);
                    throw;
                }
            }

            return formulas;
        }
        
        public IDerivedValue Calculate<T>(string name, IEnumerable<IBasicValue<T>> components) 
            where T : struct
        {
            if (this.Formulas.Any(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                string formula = this.Formulas.First(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .Value;

                int value = this.Calculate(components, formula);
                return new ConcreteDerivedIntValue(name, value, value);
            }
            
            throw new InvalidOperationException("Could not find formula for name of value " + name);
        }

        public Dictionary<string, IDerivedValue> GetEntityStandardBlock(IEnumerable<IBasicValue<int>> components)
        {
            Dictionary<string, IDerivedValue> values = new Dictionary<string, IDerivedValue>();
            foreach (KeyValuePair<string, string> pair in this.EntityStandardFormulas)
            {
                int result = Math.Max(1, this.Calculate<int>(components, pair.Value)); 
                values.Add(
                    pair.Key,
                    new ConcreteDerivedIntValue(
                        pair.Key,
                        result,
                        result));
            }

            return values;
        }

        public Dictionary<string, IDerivedValue> GetItemStandardBlock(IEnumerable<IBasicValue<float>> components)
        {
            Dictionary<string, IDerivedValue> values = new Dictionary<string, IDerivedValue>();
            foreach (KeyValuePair<string, string> pair in this.ItemStandardFormulas)
            {
                int result = Math.Max(1, this.Calculate(components, pair.Value)); 
                values.Add(
                    pair.Key,
                    new ConcreteDerivedIntValue(
                        pair.Key,
                        result,
                        result));
            }

            return values;
        }

        public bool AddFormula(string name, string formula)
        {
            if (this.Formulas.Any(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase)) == false)
            {
                this.Formulas.Add(name, formula);
            }

            return false;
        }

        public Color GetBackgroundColour(string name)
        {
            if (this.DerivedValueBackgroundColours.ContainsKey(name))
            {
                return this.DerivedValueBackgroundColours[name];
            }
            return Color.gray;
        }
        
        public Color GetTextColour(string name)
        {
            if (this.DerivedValueTextColours.ContainsKey(name))
            {
                return this.DerivedValueTextColours[name];
            }
            return Color.gray;
        }
        
        public Color GetOutlineColour(string name)
        {
            if (this.DerivedValueOutlineColours.ContainsKey(name))
            {
                return this.DerivedValueOutlineColours[name];
            }
            return Color.gray;
        }

        public int Calculate<T>(IEnumerable<IBasicValue<T>> components, string formula)
            where T : struct
        {
            string eval = formula;
            foreach (IBasicValue<T> value in components)
            {
                eval = eval.Replace(value.Name, value.Value.ToString());
            }

            return ScriptingEngine.Instance.Evaluate<int>(eval);
        }
    }
}