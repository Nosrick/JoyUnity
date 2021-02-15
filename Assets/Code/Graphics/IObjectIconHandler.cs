using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public interface IObjectIconHandler
    {
        bool AddSpriteData(string tileSet, SpriteData dataToAdd);
        bool AddSpriteDataRange(string tileSet, IEnumerable<SpriteData> dataToAdd);
        bool AddSpriteDataFromXML(string tileSet, XElement spriteDataElement);
        bool AddSpriteDataFromJson(string tileSet, JToken spriteDataToken);
        IEnumerable<SpriteData> ReturnDefaultData();
        SpriteData ReturnDefaultIcon();
        IEnumerable<SpriteData> GetTileSet(string tileSet);
        IEnumerable<SpriteData> GetSprites(string tileSet, string tileName, string state = "DEFAULT");
        SpriteData GetFrame(string tileSet, string tileName, string state = "DEFAULT", int frame = 0);
        List<Sprite> GetRawFrames(string tileSet, string tileName, string partName, string state = "DEFAULT");
        int SpriteSize { get; }
    }
}