
namespace ProgStyles
{
    class Program
    {
        private static readonly Oop.Token[] _expression = {
            new Oop.ValueToken(5),
            new Oop.ValueToken(1), 
            new Oop.ValueToken(2), 
            new Oop.OperatorPlus(), 
            new Oop.ValueToken(4), 
            new Oop.OperatorMultiply(), 
            new Oop.OperatorPlus(), 
            new Oop.ValueToken(3), 
            new Oop.OperatorMinus(), 
        };

        private static readonly Fp.Token[] _expression2 = {
            new Fp.Token { Op = Fp.Operation.Value, Value = 5 },
            new Fp.Token { Op = Fp.Operation.Value, Value = 1 },
            new Fp.Token { Op = Fp.Operation.Value, Value = 2 }, 
            new Fp.Token { Op = Fp.Operation.Plus }, 
            new Fp.Token { Op = Fp.Operation.Value, Value = 4 }, 
            new Fp.Token { Op = Fp.Operation.Multiply }, 
            new Fp.Token { Op = Fp.Operation.Plus }, 
            new Fp.Token { Op = Fp.Operation.Value, Value = 3 }, 
            new Fp.Token { Op = Fp.Operation.Minus }
        };

        static void Main(string[] args)
        {
            var calculator = new Oop.Calculator();
            var calculator2 = Fp.Calculator.New();
            var calculator3 = Fp.Calculator2.New();

            var result = calculator.Calculate(_expression);
            var result2 = calculator2.Calculate(_expression2);
            var result3 = calculator3.Calculate(_expression2);
        }
    }
}
