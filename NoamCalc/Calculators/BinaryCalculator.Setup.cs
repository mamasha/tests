partial class BinaryCalculator
{
    public delegate decimal OpImplementation(decimal a, decimal b);

    public interface IConfigurator
    {
        IConfigurator On(string op, OpImplementation impl);
    }

    class Setup : IConfigurator
    {
        private readonly BinaryCalculator _calculator;

        public Setup(BinaryCalculator calculator)
        {
            _calculator = calculator;
        }

        IConfigurator IConfigurator.On(string op, OpImplementation impl)
        {
            _calculator.AddOperation(op, impl);
            return this;
        }
    }
}

