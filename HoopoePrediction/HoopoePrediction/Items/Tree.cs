using System;
using HoopoePrediction.Model;
using Mars.Components.Environments.Cartesian;
using Mars.Interfaces.Environments;

namespace HoopoePrediction.Items
{
    public class Tree : Objective
    {
        private LandscapeLayer Layer;
        public Tree(LandscapeLayer layer)
        {
            Layer = layer;
        }
        

        public Position Position { get; set; }
        Guid Objective.ID
        {
            get => ID;
            set => ID = value;
        }

        Position Objective.Position
        {
            get => Position;
            set => Position = value;
        }

        public Guid ID { get; set; }
        public void Tick()
        {
            throw new NotImplementedException();
        }
    }
}