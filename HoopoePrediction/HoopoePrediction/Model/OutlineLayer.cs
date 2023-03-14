using System;
using System.Linq;
using HoopoePrediction.Items;
using Mars.Common.Core.Collections.SimpleTree;
using Mars.Components.Environments;
using Mars.Components.Environments.Cartesian;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using HoopoePrediction.Items;

namespace HoopoePrediction.Model
{
    /// <summary>
    ///     A simple raster layer that provides access to the values of a raster input.
    /// </summary>
    public class OutlineLayer : RasterLayer
    {

        public bool IsPointInside(Position coordinate)
        {
            // First, check if the coordinate is inside the area of the .asc file.
            // The comparison with "1" ensures that it's not a "non-walkable" cell (the area outside of our polygon).
            // It's the NoData value that was set previously in QGIS.
            return Width>= coordinate.X && Height>=coordinate.Y && GetValue(coordinate) != 1;
        }
    }
}