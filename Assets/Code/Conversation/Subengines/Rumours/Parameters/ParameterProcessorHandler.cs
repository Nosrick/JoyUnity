using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Conversation.Subengines.Rumours.Parameters
{
    public class ParameterProcessorHandler : IParameterProcessorHandler
    {
        protected List<IParameterProcessor> Parameters { get; set; }

        public ParameterProcessorHandler()
        {
            Parameters = LoadProcessors();
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