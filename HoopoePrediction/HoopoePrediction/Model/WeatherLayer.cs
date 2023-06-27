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

        private double GetWeatherCondition(int day, int hour)
        {
            return _condition[day,hour];
        }

        public void ReadWeatherData(string path, int days)
        {
            
            string newPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
            //string newPath = GoUpPathLevel(AppDomain.CurrentDomain.BaseDirectory, 4);
            newPath = Path.GetFullPath(Path.Combine(newPath , path));
            
            string[] arrLine = File.ReadAllLines(newPath);
            var hours = 0;
            var day = 0;
            _temperature = new double[days, hoursPerDay];
            _precipitation = new double[days, hoursPerDay];
            _humidity = new double[days, hoursPerDay];
            _condition = new double[days, hoursPerDay];
            
            for (int i = 1; i < arrLine.Length; i++)
            {
                var values = arrLine[i].Split(',');
                // _temperature[day, hours] = double.Parse(values[1], System.Globalization.CultureInfo.InvariantCulture);
                // _precipitation[day, hours] = double.Parse(values[4], System.Globalization.CultureInfo.InvariantCulture);
                // _humidity[day, hours] = double.Parse(values[3], System.Globalization.CultureInfo.InvariantCulture);
                _temperature[day, hours] = Convert.ToDouble(values[1]);
                _humidity[day, hours] =Convert.ToDouble(values[3]);
                _precipitation[day, hours] = Convert.ToDouble(values[4]);
                _condition[day, hours] =Convert.ToDouble(values[11]);
                ++hours;
                if (hours == 24)
                {
                    ++day;
                    hours = 0;
                }
            }
            
        }

        static string GoUpPathLevel(string path, int levels)
        {
            string currentPath = path;
        
            for (int i = 0; i < levels; i++)
            {
                currentPath = Directory.GetParent(currentPath)?.FullName;
            
                if (currentPath == null)
                    break;
            }
        
            return currentPath;
        }
    }
}