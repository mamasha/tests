namespace MsShapeEditor;

public record Point(int X, int Y);

public class Geometry
{ }

public class Circle : Geometry
{
    public Point Center { get; set; }
    public int Radius { get; set; }
}

public class Square : Geometry
{
    public Point LeftUp { get; set; }
    public int Edge { get; set; }
}

public class Color
{
    public string Value { get; set; }
}

public class IShape
{
    public string Id { get; set; }
    public IBoard Board { get; set; }
    public Color Color { get; set; }
}

public class IShape<TGeometry> : IShape
{
    public TGeometry Geometry { get; set; }
}

public class Shape<TGeometry> : IShape<TGeometry>
{
    public string Id { get; set; }
    public IBoard Board { get; set; }
    public Color Color { get; set; }
    public TGeometry Geometry { get; set; }
}
