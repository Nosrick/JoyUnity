using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Conversation.Subengines.Rumours.Parameters
{
    public class ParameterProcessorHandler : MonoBehaviour
    {
        protected List<IParameterProcessor> Parameters { get; set; }

        public void Awake()
        {
            if (Parameters is null)
            {
                Parameters = LoadProcessors();
            }
        }

        protected List<IParameterProcessor> LoadProcessors()
        {
            return new List<IParameterProcessor>(ScriptingEngine.instance.FetchAndInitialiseChildren<IParameterProcessor>());
        }

        public IParameterProcessor Get(string parameter)
        {
            return Parameters.First(p => p.CanParse(parameter));
        }
    }
}