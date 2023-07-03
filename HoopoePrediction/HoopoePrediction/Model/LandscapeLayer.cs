using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HoopoePrediction.Items;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
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
            
            Weather.ReadWeatherData(WeatherDataPath, 8);
            var temp = Weather.GetTemperature(0, 3);
            // var agentManager = layerInitData.Container.Resolve<IAgentManager>();
            // Hoopoes = agentManager.Spawn<HoopoeAgent, LandscapeLayer>().ToList();
            var layer = this;
            Environment = new SpatialHashEnvironment<Objective>(Fence.Width, Fence.Height);
            Hoopoes = new List<HoopoeAgent>();
            ApplicableSpots = new List<Position>();
            Rasterspots = new List<Position>();
            MinAmountSpots = RasterLength * RasterWidth * PercentageTiles;
            SelectApplicable();
            LogPositions(ApplicableSpots, ApplicableSpotsPath);
            var spawnpoints = Cluster();
            Console.WriteLine("Potential Habitats " +spawnpoints.Count);
            LogPositions(Rasterspots,ClusterspotsPath);
            

            //spawn agents
            for (int i = Startpoint; i < spawnpoints.Count; i++)
            {
                var spawn = Rasterspots[i];
                var agent = new HoopoeAgent();
                agent.MemberId = 0;
                agent.XSpawn = (int) spawn.X;
                agent.YSpawn = (int) spawn.Y;
                agent.XWidth = RasterWidth;
                agent.YLength = RasterLength;
                agent.MinHeight = MinHeight;
                agent.MaxHeight = MaxHeight;
                agent.Init(layer);
                registerAgentHandle.Invoke(layer,agent);
                Hoopoes.Add(agent);
            }
            //create entities
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
            
            Results = new List<List<Position>>();
            return Hoopoes.Count > 0;
        }

        /// <summary>
        /// Creates an entity of a given type at given position
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="position"></param>
        /// <param name="type"></param>
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
        /// Selects all applicablespots for a territory
        /// </summary>
        private void SelectApplicable()
        {
            var height = Fence.Height ;
            var width = Fence.Width ;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var pos = Position.CreatePosition(j, i);
                    if (CheckIfApplicable(pos))
                    {
                        ApplicableSpots.Add(pos);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Checks if a position is considered an applicablespot
        /// </summary>
        /// <param name="pos"></param>
        private bool CheckIfApplicable(Position pos)
        {
            if (checkElevation(pos))
            {
                if (Meadow.GetValue(pos)==0)
                {
                    if (Street.GetValue(pos) != 0 && Tertiary.GetValue(pos)!=0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Clusters applicablespots and creates territories aswell as spawnpoints for the agents
        /// </summary>
        /// <returns>List of spawnpoints for hoopoeagents</returns>
        private List<Position> Cluster()
        {  var rasterStartpointX = 0;
            var rasterStartpointY = 0;
            var tempList = new List<Position>();
            var streetTileList = new List<Position>();
            var treeList = new List<Position>();
            var usedPositions = new List<Position>();
            var startpoints = new List<Position>();
            var rasterEndpointX = RasterWidth;
            var rasterEndpointY = RasterLength;

            for (int y = 0; y < Fence.Height; y+=RasterLength)
            {
                rasterStartpointY = y;
                for (int x = 0; x < Fence.Width; x++) //while x<Fence.Width
                {
                    rasterStartpointX = x;
                    rasterEndpointX = x + RasterWidth;
                    for (int i = y; i < rasterEndpointY-1; i++)
                    {
                        for (int j = x; j < rasterEndpointX-1; j++)
                        {
                            var pos = Position.CreatePosition(j, i);
                            if (j < Fence.Width)
                            {
                                // Math.Abs(Applicable.GetValue(pos) - 7) < 1
                                if (CheckIfApplicable(pos) && !usedPositions.Contains(pos))
                                {
                                    tempList.Add(pos);
                                }
                                else if (Street.GetValue(pos) == 0 && Tertiary.GetValue(pos)==0)
                                {
                                    streetTileList.Add(pos);
                                }
                                else if (Trees.GetValue(pos) == 0)
                                {
                                    treeList.Add(pos);
                                }
                            }
                            
                        }
                    }

                    if (tempList.Count>= MinAmountSpots)
                    {
                        if (tempList.Count>= streetTileList.Count && treeList.Count>= TreeCount)
                        {
                            startpoints.Add(Position.CreatePosition(rasterStartpointX, rasterStartpointY));
                        
                            foreach (var pos in tempList)
                            {
                                Rasterspots.Add(pos);
                                usedPositions.Add(pos);
                            }

                            foreach (var pos in treeList)
                            {
                                usedPositions.Add(pos);
                            }
                            
                            if (x+RasterWidth<Fence.Width)
                            {
                                x += RasterWidth;
                                //rasterEndpointX += RasterWidth;
                            }
                        }
                    }
                    else
                    {
                        ++rasterEndpointX;
                    }
                    tempList.Clear();
                    streetTileList.Clear();
                    usedPositions.Clear();
                }
                rasterEndpointY+=RasterLength;
                if (rasterEndpointY>Fence.Height)
                {
                    break;
                }
            }

            return startpoints;
        }

        /// <summary>
        /// Checks if a position is within the min and max height
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>true if it is within min and max height</returns>
        private bool checkElevation(Position pos)
        {
            var currentHeight = Elevation.GetHeight(pos);
            if (currentHeight>=MinHeight && currentHeight<=MaxHeight)
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Logs the positions of the list in the file of given path
        /// by changing the number at position
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filepath"></param>
        /// <returns>true if it is within min and max height</returns>
        private static void LogPositions(List<Position> list, String filepath)
        {
            if (list.Count < 1)
            {
                return;
            }

            string newPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName);
            newPath = Path.GetFullPath(Path.Combine(newPath , filepath));
            
             
            string[] arrLine = File.ReadAllLines(newPath);
            int maxColumns = arrLine.Length;
            int startIndex = maxColumns; 
            int count = 0;
            int spotsIdx = 0;
            var replacingValueCharacter = '7';
            var valueCharacter = '0';
            var nonValueCharacter = '1';

            //90 -82 -= 7 || 90 -0 = 90 -1idx =  89
            foreach (var spot in list)
            {
                startIndex = (int) (startIndex - spot.Y - 1);
                var line = arrLine[startIndex];
                var arr = new StringBuilder(line);
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] == valueCharacter)
                    {
                        arr[i] = nonValueCharacter;
                        ++count;
                    }
                    else if (arr[i] == nonValueCharacter || arr[i] == replacingValueCharacter)
                    {
                        ++count;
                    }
                    if (Math.Abs(count - spot.X) < 1)
                    { 
                        // val pos = next value | if no value left -> break;
                        // //pos.X = -1;
                        arr[i] = replacingValueCharacter;
                        count = 0;
                        break;
                    }
                }
                arrLine[startIndex] = arr.ToString();
                startIndex = maxColumns;
            }

            for (int numTries = 0; numTries < 10; numTries++)
            {
                try
                {
                    File.WriteAllLines(newPath, arrLine);
                    //Console.WriteLine("Success");
                    break;
                }
                catch (IOException)
                {
                    //Console.WriteLine("sleep");
                }
            }

        }
        

        
        #region Properties and Fields

        private List<HoopoeAgent> Hoopoes { get; set; }
        
        [PropertyDescription(Name = "OutlineLayer")]
        public OutlineLayer Fence { get; set; }
        
        [PropertyDescription(Name = "ElevationLayer")]
        public ElevationLayer Elevation { get; set; }
        
        [PropertyDescription(Name = "MeadowLayer")]
        public MeadowLayer Meadow { get; set; }
        
        [PropertyDescription(Name = "StreetLayer")]
        public StreetLayer Street { get; set; }
        
        [PropertyDescription(Name = "TreeLayer")]
        public TreeLayer Trees { get; set; }
        
        [PropertyDescription(Name = "TertiaryLayer")]
        public TertiaryLayer Tertiary { get; set; }
        
        [PropertyDescription(Name = "WeatherLayer")]
        public WeatherLayer Weather { get; set; }
        
        [PropertyDescription(Name = "ResultFilePath")] public String ResultFilePath { get; set; }
        
        [PropertyDescription(Name = "ApplicableSpotsPath")] public String ApplicableSpotsPath { get; set; }
        
        [PropertyDescription(Name = "ClusterspotsPath")] public String ClusterspotsPath { get; set; }

        [PropertyDescription(Name = "WeatherDataPath")] public String WeatherDataPath { get; set; }

        [PropertyDescription(Name = "RasterWidth")] public int RasterWidth { get; set; }
        
        [PropertyDescription(Name = "RasterLength")] public int RasterLength { get; set; }

        [PropertyDescription(Name = "Startpoint")] public int Startpoint { get; set; }
        
        [PropertyDescription(Name = "PercentageTiles")] public double PercentageTiles { get; set; }
        
        [PropertyDescription(Name = "MinHeight")] public int MinHeight { get; set; }
        
        [PropertyDescription(Name = "MaxHeight")] public int MaxHeight { get; set; }
        
        [PropertyDescription(Name = "TreeCount")] public int TreeCount { get; set; }
        
        public SpatialHashEnvironment<Objective> Environment { get; set; }

        public List<List<Position>> Results;
        
        public List<Position> ApplicableSpots;

        public double MinAmountSpots;

        public List<Position> Rasterspots;

        public int SuccessRate;

        public enum LandscapeType
        {
            Grass = 0,
            Tree = 1,
            Street = 2
        }
        
        #endregion
    }
}