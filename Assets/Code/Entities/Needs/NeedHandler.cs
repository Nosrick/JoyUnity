using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Needs
{
    public class NeedHandler : INeedHandler
    {
        protected Dictionary<string, INeed> m_NeedsMasters;

        public NeedHandler()
        {
            this.m_NeedsMasters = this.Load().ToDictionary(need => need.Name, need => need);
        }

        public IEnumerable<INeed> Load()
        {
            try
            {
                return Scripting.ScriptingEngine.Instance.FetchAndInitialiseChildren<INeed>().ToList();
            }
            catch(Exception ex)
            {
                GlobalConstants.ActionLog.StackTrace(ex);
                return new List<INeed>();
            }
        }

        protected static Dictionary<string, INeed> Initialise()
        {
            try
            {
                Dictionary<string, INeed> needs = new Dictionary<string, INeed>();

                IEnumerable<INeed> needTypes = Scripting.ScriptingEngine.Instance.FetchAndInitialiseChildren<INeed>();

                foreach (INeed type in needTypes)
                {
                    needs.Add(type.Name, type);
                }
                return needs;
            }
            catch(Exception ex)
            {
                GlobalConstants.ActionLog.StackTrace(ex);
                return new Dictionary<string, INeed>();
            }
        }

        public INeed Get(string name)
        {
            if(this.m_NeedsMasters is null)
            {
                this.m_NeedsMasters = Initialise();
            }

            if(this.m_NeedsMasters.ContainsKey(name))
            {
                return this.m_NeedsMasters[name].Copy();
            }
            throw new InvalidOperationException("Need not found, looking for " + name);
        }

        public ICollection<INeed> GetMany(IEnumerable<string> names)
        {
            if (this.m_NeedsMasters is null)
            {
                this.m_NeedsMasters = Initialise();
            }

            INeed[] needs = this.m_NeedsMasters
                .Where(pair => names.Any(
                    name => name.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value)
                .ToArray();

            return needs;
        }

        public ICollection<INeed> GetManyRandomised(IEnumerable<string> names)
        {
            INeed[] tempNeeds = this.GetMany(names).ToArray();

            List<INeed> needs = new List<INeed>();

            foreach (INeed need in tempNeeds)
            {
                needs.Add(need.Randomise());
            }

            return needs;
        }

        public INeed GetRandomised(string name)
        {
            if(this.m_NeedsMasters is null)
            {
                this.m_NeedsMasters = Initialise();
            }

            if(this.m_NeedsMasters.ContainsKey(name))
            {
                return this.m_NeedsMasters[name].Randomise();
            }
            throw new InvalidOperationException("Need not found, looking for " + name);
        }

        public IEnumerable<INeed> Values
        {
            get
            {
                if (this.m_NeedsMasters is null)
                {
                    this.m_NeedsMasters = Initialise();
                }
                return new List<INeed>(this.m_NeedsMasters.Values);
            }
        }

        public IEnumerable<string> NeedNames
        {
            get
            {
                if (this.m_NeedsMasters is null)
                {
                    this.m_NeedsMasters = Initialise();
                }

                return new List<string>(this.m_NeedsMasters.Keys);
            }
        }

        public void Dispose()
        {
            string[] keys = this.m_NeedsMasters.Keys.ToArray();
            foreach (string key in keys)
            {
                this.m_NeedsMasters[key] = null;
            }

            this.m_NeedsMasters = null;
        }

        ~NeedHandler()
        {
            this.Dispose();
        }
    }
}
