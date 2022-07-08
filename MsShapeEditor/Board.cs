using Newtonsoft.Json.Linq;

namespace MsShapeEditor;

public interface IBoard
{
    string Process(Command cmd);
    string Undo();
}

class Board : IBoard
{
    private readonly IRepository _repo;
    private readonly IUndo _undo;

    public Board(IRepository repo, IUndo undo)
    {
        _repo = repo;
        _undo = undo;
    }

    private IShape? Process(Command cmd)
    {
        var shape = cmd switch
        {
            InsertCmd insert => insert.Geometry switch
            {
                Circle circle => new Shape<Circle>() { Geometry = circle },
                Square square => new Shape<Square>() { Geometry = square },
                _ => throw new NotImplementedException()
            },
            UpdateCmd update => _repo.Get(update.ShapeId),
            RemoveCmd remove => _repo.Get(remove.ShapeId),
            _ => throw new NotImplementedException()
        };

        return shape;
    }

    private Command MakeUndo(IShape shape, Command cmd)
    {
        return default;
    }

    string IBoard.Process(Command cmd)
    {
        var shape = Process(cmd);
        var undo = MakeUndo(shape, cmd);

        _undo.Push(undo);

        return shape.Id;
    }

    string IBoard.Undo()
    {
        var undo = _undo.Pop();
        var shape = Process(undo);

        return shape.Id;
    }
}
