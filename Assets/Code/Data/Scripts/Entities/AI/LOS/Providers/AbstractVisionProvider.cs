using UnityEngine;
using System;
using System.Linq;
using JoyLib.Code.World;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public class AbstractVisionProvider : IVision
    {
        public string Name => "abstractvisionprovider";

        protected bool[,] m_Vision;

        protected AbstractVisionProvider(IFOVHandler algorithm)
        {
            Algorithm = algorithm;
        }

        public virtual bool CanSee(Entity viewer, WorldInstance world, int x, int y)
        {
            return m_Vision[x, y];
        }

        public virtual bool CanSee(Entity viewer, WorldInstance world, Vector2Int point)
        {
            return m_Vision[point.x, point.y];
        }

        public virtual bool HasVisibility(Entity viewer, WorldInstance world, int x, int y)
        {
            return CanSee(viewer, world, x, y);
        }

        public virtual bool HasVisibility(Entity viewer, WorldInstance world, Vector2Int point)
        {
            return CanSee(viewer, world, point.x, point.y);
        }

        public virtual RectInt GetFullVisionRect(Entity viewer)
        {
            RectInt visionRect = new RectInt(0, 0, viewer.Vision.GetLength(0), viewer.Vision.GetLength(1));
            return visionRect;
        }

        public virtual Vector2Int[] GetVisibleWalls(Entity viewer, WorldInstance world)
        {
            Vector2Int[] visibleWalls = viewer.MyWorld.Walls.Where(
                wall => viewer.VisionProvider.CanSee(viewer, world, wall.Key))
                .ToDictionary(wall => wall.Key, wall => wall.Value).Keys.ToArray();
            return visibleWalls;
        }

        public virtual RectInt GetVisionRect(Entity viewer)
        {
            RectInt visionRect = new RectInt(viewer.WorldPosition, new Vector2Int(viewer.VisionMod, viewer.VisionMod));
            return visionRect;
        }

        public virtual void Update(Entity viewer, WorldInstance world)
        {
            throw new NotImplementedException("Someone forgot to implement Update()");
        }

        public bool[,] Vision 
        { 
            get => m_Vision;
            protected set => m_Vision = value;
        }

        protected IFOVHandler Algorithm
        {
            get;
            set;
        }

        protected FOVBasicBoard Board
        {
            get;
            set;
        }
    }
}