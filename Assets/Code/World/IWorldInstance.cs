using System;
using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.World.Lighting;
using UnityEngine;

namespace JoyLib.Code.World
{
    public interface IWorldInstance : ITagged
    {
        WorldTile[,] Tiles { get; }
        byte[,] Costs { get; }
        LightCalculator LightCalculator { get; }
        Dictionary<Vector2Int, IWorldInstance> Areas { get; }
        List<IEntity> Entities { get; }
        List<IJoyObject> Objects { get; }
        Dictionary<Vector2Int, IJoyObject> Walls { get; }
        Vector2Int SpawnPoint { get; set; }
        IWorldInstance Parent { get; set; }
        long GUID { get; }
        string Name { get; }
        IEntity Player { get; }
        Vector2Int Dimensions { get; }
        bool IsDirty { get; }

        void SetDateTime(DateTime dateTime);
        void AddObject(IJoyObject objectRef);
        bool RemoveObject(Vector2Int positionRef, IItemInstance itemRef);
        IJoyObject GetObject(Vector2Int WorldPosition);
        void Tick();
        IEnumerable<IJoyObject> SearchForObjects(IEntity entityRef, IEnumerable<string> tags);
        IEnumerable<IEntity> SearchForEntities(IEntity actor, IEnumerable<string> searchCriteria);
        IEntity GetRandomSentient();
        IEntity GetRandomSentientWorldWide();
        List<IWorldInstance> GetWorlds(IWorldInstance parent);
        Vector2Int GetTransitionPointForParent();
        IWorldInstance GetOverworld();
        IWorldInstance GetPlayerWorld(IWorldInstance parent);
        void SwapPosition(IEntity left, IEntity right);
        IItemInstance PickUpObject(IEntity entityRef);
        void AddEntity(IEntity entityRef);
        void RemoveEntity(Vector2Int positionRef);
        IEntity GetEntity(Vector2Int positionRef);
        Sector GetSectorFromPoint(Vector2Int point);
        List<Vector2Int> GetVisibleWalls(IEntity viewer);
        Dictionary<Vector2Int, IJoyObject> GetObjectsOfType(string[] tags);
        void AddArea(Vector2Int key, IWorldInstance value);
    }
}