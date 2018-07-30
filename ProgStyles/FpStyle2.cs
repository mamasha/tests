using System;
using System.Collections.Generic;

namespace ProgStyles {
namespace Fp
{
    class Calculator2 : ICalculator
    {
        delegate decimal Implementation(decimal left, decimal right);
    
        private static readonly Dictionary<Operation, Implementation> _implementations = 
        new Dictionary<Operation, Implementation> {
            {Operation.Plus, (l, r) => l + r },
            {Operation.Minus, (l, r) => l - r },
            {Operation.Multiply, (l, r) => l * r },
            {Operation.Divide, (l, r) => l / r }
        };
    
        public static ICalculator New()
        {
            return new Calculator2();
        }
        
        private Calculator2()
        {}

        decimal ICalculator.Calculate(Token[] expression)
        {
            var stack = new Stack<Token>();
            
            foreach (var token in expression)
            {
                if (token.IsValue())
                {
                    stack.Push(token);
                    continue;
                }
                
                var right = stack.Pop().Value;
                var left = stack.Pop().Value;
                
                Implementation func;
                if (!_implementations.TryGetValue(token.Op, out func))
                    throw new NotSupportedException("Operation '{0}' is not supported".Fmt(token.Op));
                
                var result = func(left, right);
                stack.Push(new Token {Op = Operation.Value, Value = result} );
            }

            var calculationResult = stack.Pop();
            return calculationResult.Value;
        }
    }
}}