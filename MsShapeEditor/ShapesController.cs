using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MsShapeEditor;

static class JsonHelpers
{ 
    public static T? ToAnonymous<T>(this JToken token, T proto)
    {
        return token.ToObject<T>();
    }
}

[ApiController]
[Route("board/{boardName}/shapes")]
public class ShapesController : ControllerBase
{
    private readonly IBoard _board;

    public ShapesController(IBoard board)
    {
        _board = board;
    }

    private IBoard GetBoard(string boardName)
    {
        return _board;
    }

    private Geometry GetGeometry(string json)
    {
        var body = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(json, new {
            Square = new Square(),
            Circle = new Circle()
        });

        if (body?.Circle is not null)
            return body.Circle;

        if (body?.Square is not null)
            return body.Square;

        throw new BadHttpRequestException("no data");
    }

    [HttpPost("insert")]
    public async Task<IActionResult> Insert(
        [FromRoute] string boardName, 
        [FromBody] JsonElement request)
    {
        var body = request.ToString();

        var board = GetBoard(boardName);
        var geometry = GetGeometry(body);

        var shapeId = board.Process(new InsertCmd { 
            Geometry = geometry 
        });

        return Ok(shapeId);
    }

    [HttpPost("update/{shapeId}")]
    public async Task<IActionResult> Update(
        [FromRoute] string boardName,
        [FromRoute] string shapeId,
        [FromBody] JsonElement request)
    {
        var body = request.ToString();

        var board = GetBoard(boardName);
        var geometry = GetGeometry(body);

        board.Process(new UpdateCmd { 
            ShapeId = shapeId,
            Geometry = geometry 
        });

        return Ok(shapeId);
    }

    [HttpPost("remove/{shapeId}")]
    public async Task<IActionResult> Remove(
        [FromRoute] string boardName,
        [FromBody] string shapeId)
    {
        var board = GetBoard(boardName);

        board.Process(new RemoveCmd { 
            ShapeId = shapeId 
        });

        return Ok(shapeId);
    }

    [HttpPost("undo")]
    public async Task<IActionResult> Undo(
        [FromRoute] string boardName)
    {
        var board = GetBoard(boardName);

        var shapeId = board.Undo();

        return Ok(shapeId);
    }
}
