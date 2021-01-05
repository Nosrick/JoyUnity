using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public interface IObjectIconHandler
    {
        bool AddIcons(string tileSet, IconData[] data);
        Texture2D[] GetIcons(string tileSet, string tileName);
        IconData[] ReturnDefaultArray();
        IconData ReturnDefaultIcon();
        Sprite[] GetDefaultSprites();
        Sprite GetSprite(string tileSet, string tileName);
        IEnumerable<Sprite> GetSprites(string tileSet, string tileName);
        IEnumerable<Sprite> GetTileSet(string tileSet);
        int SpriteSize { get; }
    }
}