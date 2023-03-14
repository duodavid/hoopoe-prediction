using System;
using Mars.Components.Environments.Cartesian;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;

namespace HoopoePrediction.Model
{
    public interface Objective : IPositionable, IAgent
    {
        public Position Position { get; set; }
        public Guid ID { get; set; }
    }
}