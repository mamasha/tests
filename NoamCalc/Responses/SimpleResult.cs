
class SimpleResult : CalculationResponse
{
    public decimal Result { get; set; }

    public SimpleResult(decimal result)
    {
        Result = result;
    }
}

