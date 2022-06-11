
class ColoredResult : SimpleResult
{ 
    public string Color { get; set; }

    public ColoredResult(decimal result, string color)
        : base(result)
    {
        Color = color;
    }
}

