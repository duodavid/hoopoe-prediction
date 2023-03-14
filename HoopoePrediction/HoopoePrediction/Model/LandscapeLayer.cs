using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoopoePrediction.Items;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using NetTopologySuite.Geometries;
using Position = Mars.Interfaces.Environments.Position;

namespace HoopoePrediction.Model
{
   
    public class LandscapeLayer : AbstractLayer
    {
        /// <summary>
        /// The LandscapeLayer registers the hoopoes in the runtime system. In this way, the tick methods
        /// of the agents can be executed later. Then the expansion of the simulation area is calculated using
        /// the raster layers described in config.json. An environment is created with this bounding box.
        /// </summary>
        /// <param name="layerInitData"></param>
        /// <param name="registerAgentHandle"></param>
        /// <param name="unregisterAgentHandle"></param>
        /// <returns>true if the agents where registered</returns>
        public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle)
        {
            base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
            
            // var agentManager = layerInitData.Container.Resolve<IAgentManager>();
            // Hoopoes = agentManager.Spawn<HoopoeAgent, LandscapeLayer>().ToList();
            var layer = this;
            Environment = new SpatialHashEnvironment<Objective>(Fence.Width, Fence.Height);

            Hoopoes = new List<HoopoeAgent>();
            var spawnpoints = CreateSpawnpoint(RasterWidth, RasterLength, Fence.Width, Fence.Height, MinRasterSize,
                Startpoint, AgentCount);
            
            for (int i = 0; i < AgentCount; i++)
            {
                var spawn = spawnpoints[i];
                var agent = new HoopoeAgent();
                agent.MemberId = spawn[0];
                agent.XSpawn = spawn[1];
                agent.YSpawn = spawn[2];
                agent.XWidth = spawn[3];
                agent.YLength = spawn[4];
                agent.MinHeight = MinHeight;
                agent.MaxHeight = MaxHeight;
                agent.PercentageTiles = PercentageTiles;
                agent.TreeCount = TreeCount;
                agent.Init(layer);
                registerAgentHandle.Invoke(layer,agent);
                Hoopoes.Add(agent);
            }
            
            Results = new List<List<Position>>();
            
            for (var x = 0; x < Fence.Width; x++)
            for (var y = 0; y < Fence.Height; y++)
            {
                var enable = Meadow[x, y];
                var position = Position.CreatePosition(x, y);
                CreateEntity(enable, position, LandscapeType.Grass);
                enable = Trees[x, y];
                CreateEntity(enable, position, LandscapeType.Tree);
                enable = Street[x, y];
                CreateEntity(enable, position, LandscapeType.Street);
                enable = Tertiary[x, y];
                CreateEntity(enable, position, LandscapeType.Street);
            }

            return Hoopoes.Count > 0;
        }

        private void CreateEntity(double enable, Position position, LandscapeType type)
        {
            
            if (enable == 0)
            {
                if (type == LandscapeType.Grass)
                {
                    var obj = new Grass(this)
                    {
                        Position = position
                    };
                    Environment.Insert(obj);
                }
                else if (type == LandscapeType.Tree)
                {
                    var obj = new Tree(this)
                    {
                        Position = position
                    };
                    Environment.Insert(obj);
                }
                else
                {
                    var obj = new Street(this)
                    {
                        Position = position
                    };
                    Environment.Insert(obj);
                }

            }
           
        
        }
        
        /// <summary>
        /// Creates spawnpoints for given amount of agents and sets a raster size for them. Rastersize
        /// depend on given parameter but can be smaller given the filesize aswell as the minFieldCovered
        /// </summary>
        /// <param name="rasterWidth">width of raster</param>
        /// <param name="rasterLength">length of raster</param>
        /// <param name="fileWidth">width of file</param>
        /// <param name="fileLength">length of file</param>
        /// <param name="minFieldCovered">mininum length of raster dimension to be considered eligble</param>
        /// <param name="startpoint">startpoint</param>
        /// <param name="agentCount">amount of agents spanpoints to create</param>
        /// <returns></returns>
        private List<int[]> CreateSpawnpoint(int rasterWidth , int rasterLength, int fileWidth, int fileLength, int minFieldCovered, int startpoint, int agentCount)
        {
            var list = new List<int[]>();
            var count = 0;
            var maxAgents = 2000; //maximale Anzahl an Agenten
            var MinFields = minFieldCovered; //mindest Anzahl an Feldern, die es braucht um in Betracht gezogen zu werden
            //var id = 0;
            var xspawn = 0;
            var yspawn = 0;
            var gender = "m";
            var width = rasterWidth; //Breite des Rasters
            var length = rasterLength; //LÄnge des Rasters
            var maxWidth = fileWidth;
            var maxLength = fileLength;
            var temp = width;
            var currentWidth = 0;
            var currentLength = 0;

            
            for (int id = 0; id < maxAgents; id++)
            {
                //Console.WriteLine(id+","+xspawn+","+yspawn+","+gender+","+width+","+length);
                
                //list.Add(id+","+xspawn+","+yspawn+","+gender+","+width+","+length);

                if (count==agentCount)
                {
                    break;
                }
                if (id>=startpoint)
                {
                    var arr = new int[5];
                    arr[0] = id;
                    arr[1] = xspawn;
                    arr[2] = yspawn;
                    arr[3] = width;
                    arr[4] = length;
                    list.Add(arr);
                    count++;
                }
                
                   
                if (width!=temp)
                {
                    width = temp;
                }

                currentWidth = maxWidth - (width + xspawn);
                
                if (currentWidth >= MinFields)
                {
                    xspawn += width;
                    if (currentWidth < width)
                    {
                        width = maxWidth - xspawn; //maxWidth - (width + xspawn);
                    }
                    
                }
                else
                {
                    currentLength = maxLength - (length+yspawn);
                    if (currentLength >= MinFields)
                    {
                        xspawn = 0;
                        yspawn += length;
                        if (currentLength < length)
                        {
                            length = currentLength;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return list;
        }
        
        
        #region Properties and Fields

        private List<HoopoeAgent> Hoopoes { get; set; }
        
        [PropertyDescription(Name = "MyGridLayer")]
        public MyGridLayer Fence { get; set; }
        
        [PropertyDescription(Name = "ElevationLayer")]
        public ElevationLayer Elevation { get; set; }
        
        [PropertyDescription(Name = "MeadowLayer")]
        private MeadowLayer Meadow { get; set; }
        
        [PropertyDescription(Name = "StreetLayer")]
        private StreetLayer Street { get; set; }
        
        [PropertyDescription(Name = "TreeLayer")]
        private TreeLayer Trees { get; set; }
        
        [PropertyDescription(Name = "TertiaryLayer")]
        private TertiaryLayer Tertiary { get; set; }
        
        [PropertyDescription(Name = "ResultFilePath")] public String ResultFilePath { get; set; }
        
        [PropertyDescription(Name = "AgentCount")] public int AgentCount { get; set; }
        
        [PropertyDescription(Name = "RasterWidth")] public int RasterWidth { get; set; }
        
        [PropertyDescription(Name = "RasterLength")] public int RasterLength { get; set; }
        
        [PropertyDescription(Name = "MinRasterSize")] public int MinRasterSize { get; set; }
        
        [PropertyDescription(Name = "Startpoint")] public int Startpoint { get; set; }
        
        [PropertyDescription(Name = "PercentageTiles")] public double PercentageTiles { get; set; }
        
        [PropertyDescription(Name = "MinHeight")] public int MinHeight { get; set; }
        
        [PropertyDescription(Name = "MaxHeight")] public int MaxHeight { get; set; }
        
        [PropertyDescription(Name = "TreeCount")] public int TreeCount { get; set; }
        
        public SpatialHashEnvironment<Objective> Environment { get; set; }

        public List<List<Position>> Results;

        public enum LandscapeType
        {
            Grass = 0,
            Tree = 1,
            Street = 2
        }
        
        #endregion
    }
}