using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.World
{
    public class StandardWorldTiles
    {
        private static Lazy<StandardWorldTiles> lazy = new Lazy<StandardWorldTiles>(() => new StandardWorldTiles());

        public static StandardWorldTiles instance = lazy.Value;

        protected HashSet<WorldTile> m_StandardTypes;

        public StandardWorldTiles()
        {
            m_StandardTypes = new HashSet<WorldTile>();
        }

        public bool AddType(WorldTile tile)
        {
            return m_StandardTypes.Add(tile);
        }

        public IEnumerable<WorldTile> GetByTileSet(string tileSet)
        {
            return m_StandardTypes.Where(x => x.TileSet.Equals(tileSet.ToLower()));
        }

        public IEnumerable<WorldTile> GetByTileName(string tileName)
        {
            return m_StandardTypes.Where(x => x.TileName.Equals(tileName.ToLower()));
        }

        public WorldTile GetSpecificTile(string tileSet, string tileName)
        {
            return m_StandardTypes.First(x => x.TileSet.Equals(tileSet.ToLower()) && x.TileName.Equals(tileName.ToLower()));
        }
        
        public IEnumerable<WorldTile> GetByTag(string tag)
        {
            return m_StandardTypes.Where(x => x.Tags.Contains(tag.ToLower()));
        }

        public IEnumerable<WorldTile> GetByTags(IEnumerable<string> tags)
        {
            return m_StandardTypes.Where(x => x.Tags.Intersect(tags).ToArray().Length > 0);
        }
    }
}