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
        
        protected Dictionary<string, Type> DerivedValueTypes { get; set; } 
        protected Dictionary<string, Color> DerivedValueColours { get; set; }
        
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
            
            this.DerivedValueColours = new Dictionary<string, Color>();
            this.EntityStandardFormulas = this.LoadFormulasFromFile(this.ENTITY_FILE);
            this.ItemStandardFormulas = this.LoadFormulasFromFile(this.ITEM_FILE);
            this.Formulas = new Dictionary<string, string>(this.EntityStandardFormulas);
            foreach (KeyValuePair<string, string> pair in this.ItemStandardFormulas)
            {
                this.Formulas.Add(pair.Key, pair.Value);
            }

            if (StatisticHandler is null == false)
            {
                Type[] assemblyTypes = StatisticHandler.GetType().Assembly.GetTypes();
                this.DerivedValueTypes = new Dictionary<string, Type>();
                List<Type> types = assemblyTypes.Where(type => type.IsAssignableFrom(typeof(IDerivedValue<int>)))
                    .ToList();
                
                types.AddRange(assemblyTypes.Where(type => type.IsAssignableFrom(typeof(IDerivedValue<float>))));

                foreach (Type type in types)
                {
                    this.DerivedValueTypes.Add(type.Name, type);
                }
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
                    string colourCode = dv.Element("Colour").DefaultIfEmpty("888888FF");
                    Color colour = new Color();
                    ColorUtility.TryParseHtmlString(colourCode, out colour);
                    formulas.Add(
                        name,
                        dv.Element("Formula").GetAs<string>().ToLower()); 
                    DerivedValueColours.Add(
                        name,
                        colour);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    throw;
                }
            }

            return formulas;
        }
        
        public IDerivedValue<K> Calculate<T, K>(string name, IEnumerable<IBasicValue<T>> components) 
            where T : struct
            where K : struct
        {
            if (this.Formulas.Any(pair => pair.Key.Equals(name, StringComparison.Ordinal)))
            {
                string formula = this.Formulas.First(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .Value;

                string value = this.Calculate<T>(components, formula).ToString();
                IDerivedValue<K> derivedValue = this.CreateDerivedValue<K>();
                derivedValue.Name = name;
                derivedValue.SetValue(value);
                derivedValue.SetMaximum(value);
            }
            
            throw new InvalidOperationException("Could not find formula for name of value " + name);
        }

        public IDerivedValue<T> Calculate<T>(string name, IEnumerable<IBasicValue<T>> components)
            where T : struct
        {
            return this.Calculate<T, T>(name, components);
        }

        public Dictionary<string, IDerivedValue<int>> GetEntityStandardBlock(IEnumerable<IBasicValue<int>> components)
        {
            Dictionary<string, IDerivedValue<int>> values = new Dictionary<string, IDerivedValue<int>>();
            foreach (KeyValuePair<string, string> pair in this.EntityStandardFormulas)
            {
                int result = this.Calculate<int>(components, pair.Value); 
                values.Add(
                    pair.Key,
                    new ConcreteDerivedIntValue(
                        pair.Key,
                        result,
                        result));
            }

            return values;
        }

        public Dictionary<string, IDerivedValue<int>> GetItemStandardBlock(IEnumerable<IBasicValue<float>> components)
        {
            Dictionary<string, IDerivedValue<int>> values = new Dictionary<string, IDerivedValue<int>>();
            foreach (KeyValuePair<string, string> pair in this.ItemStandardFormulas)
            {
                int result = (int)Math.Ceiling(this.Calculate<float>(components, pair.Value)); 
                values.Add(
                    pair.Key,
                    new ConcreteDerivedIntValue(
                        pair.Key,
                        result,
                        result));
            }

            return values;
        }

        protected IDerivedValue<K> CreateDerivedValue<K>() 
            where K : struct
        {
            if(this.DerivedValueTypes.Any(t => t.Key.Equals(typeof(K).Name, StringComparison.Ordinal)))
            {
                Type type = this.DerivedValueTypes.First(t =>
                    t.Key.Equals(typeof(K).Name, StringComparison.OrdinalIgnoreCase)).Value;

                IDerivedValue<K> derivedValue = (IDerivedValue<K>) Activator.CreateInstance(type);

                return derivedValue;
            }
            
            throw new InvalidOperationException("Type " + typeof(K).Name + " not supported for derived values");
        }

        public bool AddFormula(string name, string formula)
        {
            if (this.Formulas.Any(pair => pair.Key.Equals(name, StringComparison.OrdinalIgnoreCase)) == false)
            {
                this.Formulas.Add(name, formula);
            }

            return false;
        }

        public Color GetColour(string name)
        {
            if (DerivedValueColours.ContainsKey(name))
            {
                return DerivedValueColours[name];
            }
            return Color.gray;
        }

        public T Calculate<T>(IEnumerable<IBasicValue<T>> components, string formula)
            where T : struct
        {
            string eval = formula;
            foreach (IBasicValue<T> value in components)
            {
                eval = eval.Replace(value.Name, value.Value.ToString());
            }

            return ScriptingEngine.instance.Evaluate<T>(eval);
        }
    }
}