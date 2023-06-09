using System;
using System.Collections.Generic;
using System.Linq;
using HoopoePrediction.Items;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;

namespace HoopoePrediction.Model
{
    /// <summary>
    ///  A simple agent stub that has an Init() method for initialization and a
    ///  Tick() method for acting in every tick of the simulation.
    /// </summary>
    public class HoopoeAgent : IAgent<LandscapeLayer>, Objective
    {

        public enum LandscapeType
        {
            Dirt = 0,
            LowGrass = 1,
            Tree = 2,
            Wall = 3
        }
        
        [PropertyDescription(Name = "xSpawn")] public int XSpawn { get; set; }
        [PropertyDescription(Name = "ySpawn")] public int YSpawn { get; set; }
        [PropertyDescription(Name = "memberId")] public int MemberId { get; set; }
        
        [PropertyDescription(Name = "gender")] public string Gender { get; set; }
        [PropertyDescription(Name = "xWidth")] public int XWidth { get; set; }
        [PropertyDescription(Name = "yLength")] public int YLength { get; set; }
        
        [PropertyDescription(Name = "PercentageTiles")] public double PercentageTiles { get; set; }
        [PropertyDescription(Name = "minHeight")] public int MinHeight { get; set; }
        [PropertyDescription(Name = "maxHeight")] public int MaxHeight { get; set; }
        [PropertyDescription(Name = "TreeCount")] public int TreeCount { get; set; }
        
        
       
        private LandscapeLayer Layer { get; set; } // provides access to the main layer of this agent
        public Guid ID { get; set; } // identifies the agent
        public Position Position { get; set; }

        private List<Position> foodspots;
        private List<Position> streettiles;
        private List<Position> home;
        
        private int minFood;
        
        private int minGrass;
        private bool foodSupply;

        
        private bool updated;
        
        public void Init(LandscapeLayer layer)
        {
            Layer = layer; // store layer for access within agent class
            Position = Position.CreatePosition(XSpawn,YSpawn);

            
            // minGrass = (int) (YLength*XWidth*PercentageTiles); // je Block ca 50qm 
            // foodspots = new List<Position>();
            // streettiles = new List<Position>();
            // home = new List<Position>();
            // foodSupply = false;
            // updated = false;
        }

    

        public void Tick()
        {

            
            // LookForFoodTiles();
            // LookForTrees();
            // LookOutForStreetTiles();
            // Move();
            // CheckTerritory();


        }


        

        /**
        * Check for the elevation level of the given position
        * @param position to check the elevation for
        */
        private bool checkElevation(Position pos)
        {
            var currentHeight = Layer.Elevation.GetHeight(pos);
            if (currentHeight>=MinHeight && currentHeight<=MaxHeight)
            {
                return true;
            }
            return false;
        }
        
        /**
        * * Looks for nearby grass tiles, that have potential to provide food and adding it to the list
        */
        private void LookForFoodTiles()
        { 
            var result = Layer.Environment.Explore(Position, 3, -1, agentInEnvironment => agentInEnvironment.GetType() == typeof(Grass)).ToList();
           var fill = true;
           foreach (var spot in result)
           {
               //var temp = Position.CreatePosition(spot.Position.X, spot.Position.Y);
               // if (!foodspots.Contains(temp))
               // {
               //     foodspots.Add(Position.CreatePosition(spot.Position.X, spot.Position.Y)); 
               // }
               if (!(spot.Position.X < XSpawn + XWidth) || !(spot.Position.Y < YSpawn + YLength) ||
                   !checkElevation(spot.Position)) continue;
               if (foodspots.Count==0 )
               {
                   foodspots.Add(Position.CreatePosition(spot.Position.X, spot.Position.Y)); 
                       
               }
               foreach (var pos in foodspots)
               {
                   var contains = foodspots.Contains(spot.Position);

                   if (!contains)
                   {
                       foodspots.Add(Position.CreatePosition(spot.Position.X, spot.Position.Y));
                       break;
                   }
               }
           }
        }
        
        /**
        * * Looks for nearby street tiles and adding it to the list
        */
        private void LookOutForStreetTiles()
        { 
            var result = Layer.Environment.Explore(Position, 3, -1, agentInEnvironment => agentInEnvironment.GetType() == typeof(Street)).ToList();
            var fill = true;
            foreach (var spot in result)
            {
                //var temp = Position.CreatePosition(spot.Position.X, spot.Position.Y);
                // if (!foodspots.Contains(temp))
                // {
                //     foodspots.Add(Position.CreatePosition(spot.Position.X, spot.Position.Y)); 
                // }
                if (!(spot.Position.X < XSpawn + XWidth) || !(spot.Position.Y < YSpawn + YLength) ||
                    !checkElevation(spot.Position)) continue;
                if (streettiles.Count==0 )
                {
                    streettiles.Add(Position.CreatePosition(spot.Position.X, spot.Position.Y)); 
                       
                }
                foreach (var pos in streettiles)
                {
                    var contains = streettiles.Contains(spot.Position);

                    if (!contains)
                    {
                        streettiles.Add(Position.CreatePosition(spot.Position.X, spot.Position.Y));
                        break;
                    }
                }
            }

        }

        /**
        * * Looks for nearby trees (-area) and adding it to the list of potential homes
        */
        private void LookForTrees()
        {
            var result = Layer.Environment.Explore(Position, 3, -1, agentInEnvironment => agentInEnvironment.GetType() == typeof(Tree)).ToList();
            if (result.Count>=1)
            {
                home.Add(Position.CreatePosition(result[0].Position.X, result[0].Position.X));
            }
        }

        
        /**
        * * Moves the Agent one step further
        */
        private void Move()
        {
            if (Position.X+1<XSpawn+XWidth && IsWithinFence(Position))
            {
                Layer.Environment.MoveTo(this, Position.CreatePosition(Position.X+1,Position.Y), 1);
            }
            else
            {
                if (Position.Y + 1 < Layer.Fence.Height-1 && Position.Y + 1 < YLength+YSpawn)
                {
                    Position = Position.CreatePosition(XSpawn, Position.Y + 1);
                }
                
            }
        }

        private bool IsWithinFence(Position pos)
        {
            return Layer.Fence.IsPointInside(pos);
        }
  
        
    }
}