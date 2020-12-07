using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public abstract class AbstractVisionProvider : IVision
    {
        public string Name => "abstractvisionprovider";

        protected HashSet<Vector2Int> m_Vision;

        protected AbstractVisionProvider(IFOVHandler algorithm)
        {
            Algorithm = algorithm;
        }

        public virtual bool CanSee(IEntity viewer, IWorldInstance world, int x, int y)
        {
            if (m_Vision is null)
            {
                Debug.Log("VISION IS NULL");
            }
            return m_Vision.Contains(new Vector2Int(x, y));
        }

        public virtual bool CanSee(IEntity viewer, IWorldInstance world, Vector2Int point)
        {
            return CanSee(viewer, world, point.x, point.y);
        }

        public virtual bool HasVisibility(IEntity viewer, IWorldInstance world, int x, int y)
        {
            return CanSee(viewer, world, x, y);
        }

        public virtual bool HasVisibility(IEntity viewer, IWorldInstance world, Vector2Int point)
        {
            return CanSee(viewer, world, point.x, point.y);
        }

        public virtual RectInt GetFullVisionRect(IEntity viewer)
        {
            RectInt visionRect = new RectInt(0, 0, viewer.MyWorld.Dimensions.x, viewer.MyWorld.Dimensions.y);
            return visionRect;
        }

        public virtual Vector2Int[] GetVisibleWalls(IEntity viewer, IWorldInstance world)
        {
            Vector2Int[] visibleWalls = viewer.MyWorld.Walls.Where(
                wall => viewer.VisionProvider.CanSee(viewer, world, wall.Key))
                .ToDictionary(wall => wall.Key, wall => wall.Value).Keys.ToArray();
            return visibleWalls;
        }

        public virtual RectInt GetVisionRect(IEntity viewer)
        {
            RectInt visionRect = new RectInt(viewer.WorldPosition, new Vector2Int(viewer.VisionMod * 2 + 1, viewer.VisionMod * 2 + 1));
            return visionRect;
        }

        public virtual void Update(IEntity viewer, IWorldInstance world)
        {
            throw new NotImplementedException("Someone forgot to implement Update()");
        }

        public HashSet<Vector2Int> Vision 
        { 
            get => m_Vision;
            protected set => m_Vision = value;
        }

        protected IFOVHandler Algorithm
        {
            get;
        }

        protected IFOVBoard Board
        {
            get;
            set;
        }
    }
}