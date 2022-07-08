namespace MsShapeEditor;

interface IRepository
{
    void Add(IShape shape);
    IShape Get(string shapeId);
    void Remove(string shapeId);
}

class Repository : IRepository
{
    void IRepository.Add(IShape shape)
    {
        throw new NotImplementedException();
    }

    IShape IRepository.Get(string shapeId)
    {
        throw new NotImplementedException();
    }

    void IRepository.Remove(string shapeId)
    {
        throw new NotImplementedException();
    }
}