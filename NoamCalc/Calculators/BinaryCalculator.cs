partial class BinaryCalculator : BaseCalculator
{
    private readonly Dictionary<string, OpImplementation> _operations;

    public BinaryCalculator(Action<IConfigurator> apply)
    {
        _operations = new Dictionary<string, OpImplementation>(StringComparer.InvariantCultureIgnoreCase);

        apply(new Setup(this));
    }

    void AddOperation(string op, OpImplementation impl)
    {
        _operations.Add(op, impl);
    }

    protected override CalculationResponse CalculateAny(CalculationRequest calculation)
    {
        var request = calculation as BinaryOperation;

        if (request == null)
            throw new NotSupportedException(calculation.GetType().Name);

        return Calculate(request);
    }

    public SimpleResult Calculate(BinaryOperation request)
    {
        if (!_operations.TryGetValue(request.Op, out var impl))
            throw new NotSupportedException($"I don't know how calculate '{request.Op}'");

        var result = impl(request.A, request.B);

        return new SimpleResult(result);
    }
}

