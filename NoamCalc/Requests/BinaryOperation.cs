
class BinaryOperation : CalculationRequest
{ 
    public decimal A { get; set; }
    public decimal B { get; set; }
    public string Op { get; set; }

    public BinaryOperation(decimal a, decimal b, string op)
    {
        A = a;
        B = b;
        Op = op;
    }
}

