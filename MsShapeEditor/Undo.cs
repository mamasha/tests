namespace MsShapeEditor;

public interface IUndo
{
    void Push(Command cmd);
    Command Pop();
}

class Undo : IUndo
{
    Command IUndo.Pop()
    {
        return default;
    }

    void IUndo.Push(Command cmd)
    {
    }
}