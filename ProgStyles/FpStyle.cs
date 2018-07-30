using System;
using System.Collections.Generic;

namespace ProgStyles {
namespace Fp
{
    class Calculator : ICalculator
    {
        public static ICalculator New()
        {
            return
                new Calculator();
        }
        
        private Calculator()
        { }
    
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
                
                decimal result;
                
                switch (token.Op)
                {
                    case Operation.Plus:
                        result = left + right;
                        break;
                    case Operation.Minus:
                        result = left - right;
                        break;
                    case Operation.Multiply:
                        result = left * right;
                        break;
                    case Operation.Divide:
                        result = left / right;
                        break;
                    default:
                        throw new NotSupportedException("Operation '{0}' is not supported".Fmt(token.Op));
                }

                stack.Push(new Token { Op = Operation.Value, Value = result });
            }

            var calculationResult = stack.Pop();
            return calculationResult.Value;
        }
    }
}}