interface ICalculator
{
    CalculationResponse CalculateAny(CalculationRequest request);
}

abstract class BaseCalculator : ICalculator
{
    protected abstract CalculationResponse CalculateAny(CalculationRequest request);

    CalculationResponse ICalculator.CalculateAny(CalculationRequest request)
    {
        return CalculateAny(request);
    }
}

