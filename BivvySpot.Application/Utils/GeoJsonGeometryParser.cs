using BivvySpot.Application.Abstractions.Infrastructure;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace BivvySpot.Application.Utils;

public class GeoJsonGeometryParser : IGeometryParser
{
    private readonly GeometryFactory _gf = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
    private readonly GeoJsonReader _reader = new();

    public Point? BuildPoint(double? lat, double? lon)
        => (lat is null || lon is null) ? null : _gf.CreatePoint(new Coordinate(lon.Value, lat.Value));

    public Polygon? ParsePolygon(string? geoJson)
    {
        if (string.IsNullOrWhiteSpace(geoJson)) return null;
        var g = _reader.Read<Geometry>(geoJson);
        if (g is Polygon p) return p;
        if (g is MultiPolygon mp && mp.Count > 0) return (Polygon)mp.Geometries[0];
        throw new ArgumentException("Boundary must be Polygon or MultiPolygon.");
    }
}