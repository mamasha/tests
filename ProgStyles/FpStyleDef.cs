namespace ProgStyles {
namespace Fp
{
    enum Operation
    {
        Value, 
        Plus, Minus, Multiply, Divide
    }

    class Token
    {
        public Operation Op { get; set; }
        public decimal Value { get; set; }
    }
    
    interface ICalculator
    {
        decimal Calculate(Token[] expression);
    }

    static class TokenExtensions
    {
        public static bool IsValue(this Token token)
        {
            return
                token.Op == Operation.Value;
        }
    }
}}