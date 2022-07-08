namespace MsShapeEditor;


public class Command
{
}

public class InsertCmd : Command 
{
    public Geometry Geometry { get; set; }
}

public class UpdateCmd : Command
{
    public string ShapeId { get; set; }
    public Geometry Geometry { get; set; }
}

public class RemoveCmd : Command 
{ 
    public string ShapeId { get; set; }
}

