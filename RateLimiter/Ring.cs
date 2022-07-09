class Ring<T>
{
    private readonly T[] _arr;

    private int _head;
    private int _tail;

    public Ring(int capacity)
    {
        _arr = new T[capacity];
    }

    private int at(int indx) => indx % _arr.Length;

    public int Count => _tail - _head;
    public bool IsEmpty => Count <= 0;
    public bool IsFull => Count >= _arr.Length;

    private void ValidateNotEmpty()
    {
        if (IsEmpty)
            throw new Exception("Ring is empty");
    }

    private void ValidateNotFull()
    {
        if (IsFull)
            throw new Exception("Ring is full");
    }

    public T Next
    {
        get
        {
            ValidateNotEmpty();
            return _arr[at(_head)];
        }
    }

    public void Pop()
    {
        ValidateNotEmpty();
        _head++;
    }

    public void Push(T value)
    {
        ValidateNotFull();
        _arr[at(_tail++)] = value;
    }
}
