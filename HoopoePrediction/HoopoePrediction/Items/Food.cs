using System;
using Mars.Components.Environments.Cartesian;

namespace HoopoePrediction.Items
{
    public abstract class Food : IObstacle
    {
        public Guid ID { get; set; }
        public bool IsRoutable(ICharacter character)
        {
            throw new NotImplementedException();
        }

        public CollisionKind? HandleCollision(ICharacter character)
        {
            throw new NotImplementedException();
        }

        public VisibilityKind? HandleExploration(ICharacter explorer)
        {
            throw new NotImplementedException();
        }
    }
}