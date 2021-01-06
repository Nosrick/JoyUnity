using System.Collections.Generic;
using System.Xml.Linq;

namespace JoyLib.Code.Graphics
{
    public interface IObjectIconHandler
    {
        bool AddSpriteData(string tileSet, SpriteData dataToAdd);
        bool AddSpriteDataRange(string tileSet, IEnumerable<SpriteData> dataToAdd);
        bool AddSpriteDataFromXML(string tileSet, XElement spriteDataElement);
        IEnumerable<SpriteData> ReturnDefaultData();
        SpriteData ReturnDefaultIcon();
        IEnumerable<SpriteData> GetTileSet(string tileSet);
        IEnumerable<SpriteData> GetSprites(string tileSet, string tileName);
        SpriteData GetFrame(string tileSet, string tileName, int frame);
        int SpriteSize { get; }
    }
}