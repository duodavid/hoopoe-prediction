using Mars.Components.Layers;
using Mars.Interfaces.Environments;

namespace HoopoePrediction.Model
{
 
    public class ElevationLayer : RasterLayer
    {
        /*
         * Get height of a specific postion
         * @param the position to check
         * @return the height of the position 
        */
        public double GetHeight(Position coordinate)
        {
            // First, check if the coordinate is inside the area of the .asc file.
            // The comparison with "1" ensures that it's not a "non-walkable" cell (the area outside of our polygon).
            // It's the NoData value that was set previously in QGIS.
            var temp = GetValue(coordinate);
            return temp;
        }

    }
}