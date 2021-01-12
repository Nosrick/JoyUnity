using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Helpers;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public class VisionProviderHandler : IVisionProviderHandler
    {
        protected Dictionary<string, IVision> VisionTypes { get; set; }

        public VisionProviderHandler()
        {
            this.LoadVisionTypes();
        }

        protected void LoadVisionTypes()
        {
            if (this.VisionTypes.IsNullOrEmpty() == false)
            {
                return;
            }
            this.VisionTypes = new Dictionary<string, IVision>();

            string[] files = Directory.GetFiles(
                Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Vision Types",
                "*.xml",
                SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    XElement doc = XElement.Load(file);
                    foreach (XElement visionType in doc.Elements("VisionType"))
                    {
                        try
                        {
                            string name = visionType.Element("Name").GetAs<string>();
                            if (this.HasVision(name))
                            {
                                continue;
                            }

                            ColorUtility.TryParseHtmlString(
                                visionType.Element("DarkColour").DefaultIfEmpty("#000000FF"), out Color darkColour);
                            ColorUtility.TryParseHtmlString(
                                visionType.Element("LightColour").DefaultIfEmpty("#FFFFFFFF"), out Color lightColour);
                            string fovHandler = visionType.Element("Handler").DefaultIfEmpty("FOVShadowCasting");
                            IFOVHandler handler = (IFOVHandler) ScriptingEngine.Instance.FetchAndInitialise(fovHandler);
                            int minimumLightLevel = visionType.Element("MinimumLight").DefaultIfEmpty(0);
                            int maximumLightLevel = visionType.Element("MaximumLight")
                                .DefaultIfEmpty(GlobalConstants.MAX_LIGHT);
                            int minimumComfortLevel =
                                visionType.Element("MinimumComfort").DefaultIfEmpty(minimumLightLevel);
                            int maximumComfortLevel =
                                visionType.Element("MaximumComfort").DefaultIfEmpty(maximumLightLevel);
                            this.AddVision(new BaseVisionProvider(
                                darkColour,
                                lightColour,
                                handler,
                                minimumLightLevel,
                                minimumComfortLevel,
                                maximumLightLevel,
                                maximumComfortLevel,
                                name));
                        }
                        catch (Exception e)
                        {
                            GlobalConstants.ActionLog.AddText(e.Message);
                            GlobalConstants.ActionLog.AddText(e.StackTrace);
                            GlobalConstants.ActionLog.AddText("Could not parse vision type in file " + file);
                            GlobalConstants.ActionLog.AddText("Could not parse vision type in file " + file);
                        }
                    }
                }
                catch (Exception e)
                {
                    GlobalConstants.ActionLog.AddText(e.Message);
                    GlobalConstants.ActionLog.AddText(e.StackTrace);
                    GlobalConstants.ActionLog.AddText("Could not load vision types from file " + file);
                    GlobalConstants.ActionLog.AddText("Could not load vision types from file " + file);
                }
            }
        }

        public bool AddVision(
            string name, 
            Color darkColour, 
            Color lightColour, 
            IFOVHandler algorithm, 
            int minimumLightLevel = 0,
            int minimumComfortLevel = 0,
            int maximumLightLevel = GlobalConstants.MAX_LIGHT,
            int maximumComfortLevel = GlobalConstants.MAX_LIGHT)
        {
            if (this.VisionTypes.ContainsKey(name))
            {
                return false;
            }

            this.VisionTypes.Add(
                name,
                new BaseVisionProvider(
                    darkColour,
                    lightColour,
                    algorithm,
                    minimumLightLevel,
                    minimumComfortLevel,
                    maximumLightLevel,
                    maximumComfortLevel,
                    name));

            return true;
        }

        public bool AddVision(IVision vision)
        {
            if (this.VisionTypes.ContainsKey(vision.Name))
            {
                return false;
            }

            this.VisionTypes.Add(vision.Name, vision);
            return true;
        }

        public bool HasVision(string name)
        {
            return this.VisionTypes.ContainsKey(name);
        }

        public IVision GetVision(string name)
        {
            if (this.VisionTypes.ContainsKey(name))
            {
                return this.VisionTypes[name].Copy();
            }

            throw new InvalidOperationException("Could not find vision type with name " + name);
        }
    }
}