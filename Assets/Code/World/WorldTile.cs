using System.Collections.Generic;

namespace JoyLib.Code.World
{
    public class WorldTile
    {
        protected HashSet<string> m_Tags;

        public WorldTile(string tileName, string tileSet, IEnumerable<string> tags)
        {
            TileName = tileName;
            TileSet = tileSet;
            m_Tags = new HashSet<string>(tags);
        }

        public bool AddTag(string tag)
        {
            return m_Tags.Add(tag);
        }

        public bool RemoveTag(string tag)
        {
            return m_Tags.Remove(tag);
        }

        public HashSet<string> Tags
        {
            get
            {
                return new HashSet<string>(m_Tags);
            }
        }

        public string TileName
        {
            get;
        }

        public string TileSet
        {
            get;
        }
    }
}
