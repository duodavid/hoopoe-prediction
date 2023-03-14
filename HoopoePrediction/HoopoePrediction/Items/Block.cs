using System;
using HoopoePrediction.Model;
using Mars.Components.Environments.Cartesian;
using NetTopologySuite.Geometries;

namespace HoopoePrediction.Items
{
//Oberklasse aller Obstacle Objekte des Systems
//Wichtig: implementiert IObstacle

    public abstract class Block : IObstacle
    {
        public Block(OutlineLayer layer)
        {
            Battleground = layer;
        }

        protected Block()
        {
        }

        protected OutlineLayer Battleground { get; }

        //Gewichtung des Objekts
        public long ScoreValue { get; set; }

        public Geometry Geometry { get; set; }

        public Guid ID { get; set; } = Guid.NewGuid();

        public virtual bool IsRoutable(ICharacter character)
        {
            return false;
        }

        //Kollisionstyp des Obstacles 
        public virtual CollisionKind? HandleCollision(ICharacter character)
        {
            return CollisionKind.Block;
        }

        //Sichttyp des Obstacles
        public virtual VisibilityKind? HandleExploration(ICharacter explorer)
        {
            // throw new NotImplementedException();
            return VisibilityKind.Opaque;
        }
    }
}