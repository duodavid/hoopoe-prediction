using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HoopoePrediction.Model;
using Mars.Common.IO.Mapped;
using Mars.Components.Starter;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Model;

namespace HoopoePrediction
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            // the scenario consist of the model (represented by the model description)
            // an the simulation configuration (see config.json)

            // Create a new model description that holds all parts of the model, here: one agent type and one layer
            var description = new ModelDescription();
            description.AddLayer<MeadowLayer>(); // Layer representing meadow
            description.AddLayer<OutlineLayer>(); // we'll use a simple grid layer here
            description.AddLayer<ElevationLayer>(); // Layer representing the elevation
            description.AddLayer<StreetLayer>();
            description.AddLayer<TreeLayer>();
            description.AddLayer<TertiaryLayer>();
            description.AddLayer<LandscapeLayer>();

            description.AddAgent<HoopoeAgent, LandscapeLayer>(); // the agent type will be located at the grid layer

            // scenario definition
            // use config.json that provides the specification of the scenario
            var file = File.ReadAllText("config.json");
            var config = SimulationConfig.Deserialize(file);

            // Create simulation task accordingly
            var task = SimulationStarter.Start(description, config);

            // Run simulation
            var loopResults = task.Run();

            // Layer
            var layer = (LandscapeLayer) loopResults.Model.Layers.Values.Last();
            UpdateStats(layer.Results, layer.ResultFilePath);


            // Feedback to user that simulation run was successful
            Console.WriteLine($"Simulation execution finished after {loopResults.Iterations} steps");
        }

        private static void UpdateStats(List<List<Position>> list, String filepath)
        {
            if (list.Count < 1)
            {
                return;
            }


            string[] arrLine = File.ReadAllLines(filepath);
            int maxColumns = arrLine.Length;
            int startIndex = maxColumns; // 511 - 503 + 1 = 7 || 90 - 83 +1 = 6
            int count = 0;
            int spotsIdx = 0;


            //90 -82 -= 7 || 90 -0 = 90 -1idx =  89
            foreach (var spotlist in list)
            {
                foreach (var spot in spotlist)
                {
                    startIndex = (int) (startIndex - spot.Y - 1);
                    var line = arrLine[startIndex];
                    var arr = new StringBuilder(line);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (arr[i] == '1' || arr[i] == '0' || arr[i] == '9')
                        {
                            ++count;
                        }

                        if (Math.Abs(count - spot.X) < 1)
                        {
                            // val pos = next value | if no value left -> break;
                            //pos.X = -1;
                            arr[i] = '9';
                            count = 0;
                            break;
                        }
                    }

                    arrLine[startIndex] = arr.ToString();
                    startIndex = maxColumns;
                }
            }


            for (int numTries = 0; numTries < 10; numTries++)
            {
                try
                {
                    File.WriteAllLines(filepath, arrLine);
                    //Console.WriteLine("Success");
                    break;
                }
                catch (IOException)
                {
                    //Console.WriteLine("sleep");
                }
            }
        }
    }
}