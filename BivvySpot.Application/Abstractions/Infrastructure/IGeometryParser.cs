using NetTopologySuite.Geometries;

namespace BivvySpot.Application.Abstractions.Infrastructure;

public interface IGeometryParser
{
    Point? BuildPoint(double? lat, double? lon);         // returns SRID 4326 or null
    Polygon? ParsePolygon(string? geoJson);
}