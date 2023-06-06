using System;
using System.IO;
using Mars.Components.Layers;
using Mars.Interfaces.Environments;

namespace HoopoePrediction.Model
{
    public class WeatherLayer : RasterLayer
    {
        private static int hoursPerDay = 24;
        
        //Tag und Stunden
        private double[,] _temperature;
        
        private double[,] _precipitation;
        
        private double[,] _humidity;

        private double[,] _condition;
        

        public double GetPrecipitation(int day, int hour)
        {
            return _temperature[day, hour];
        }
        public double GetTemperature(int day, int hour)
        {
            return _precipitation[day, hour];
        }
        public double GetHumidty(int day, int hour)
        {
            return _humidity[day, hour];
        }

        public void ReadWeatherData(string path, int days)
        {
            
            string[] arrLine = File.ReadAllLines(path);
            var hours = 0;
            var day = 0;
            _temperature = new double[days, hoursPerDay];
            _precipitation = new double[days, hoursPerDay];
            _humidity = new double[days, hoursPerDay];

            for (int i = 1; i < arrLine.Length; i++)
            {
                var values = arrLine[i].Split(',');
                // _temperature[day, hours] = double.Parse(values[1], System.Globalization.CultureInfo.InvariantCulture);
                // _precipitation[day, hours] = double.Parse(values[4], System.Globalization.CultureInfo.InvariantCulture);
                // _humidity[day, hours] = double.Parse(values[3], System.Globalization.CultureInfo.InvariantCulture);
                _temperature[day, hours] = Convert.ToDouble(values[1]);
                _precipitation[day, hours] = Convert.ToDouble(values[3]);
                _humidity[day, hours] =Convert.ToDouble(values[4]);
                ++hours;
                if (hours == 24)
                {
                    ++day;
                    hours = 0;
                }
            }
            // using(var reader = new StreamReader(@path))
            // {
            //     List<string> listA = new List<string>();
            //     List<string> listB = new List<string>();
            //     while (!reader.EndOfStream)
            //     {
            //         
            //         
            //         var line = reader.ReadLine();
            //         var values = line.Split(';');
            //     
            //         listA.Add(values[0]);
            //         listB.Add(values[1]);
            //     }
            // }
        }

        
    }
}