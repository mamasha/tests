using System;
using System.Collections.Generic;

namespace ProgStyles {
namespace Oop 
{
    abstract class BaseCalculator
    {
        public abstract decimal Calculate(Token[] expression);
    }

    class Calculator : BaseCalculator
    {
        public Calculator()
        { }

        public override decimal Calculate(Token[] expression)
        {
            var stack = new Stack<Token>();

            foreach (var token in expression)
            {
                if (token.IsValue)
                {
                    stack.Push(token);
                }
                else
                {
                    var right = stack.Pop().Value;
                    var left = stack.Pop().Value;

                    var result = (token as OperatorToken).Calculate(left, right);

                    stack.Push(new ValueToken(result));
                }
            }

            if (stack.Count != 1)
                throw new InvalidOperationException();

            var calculationResult = stack.Pop();

            if (!calculationResult.IsValue)
                throw new InvalidOperationException();

            return calculationResult.Value;
        }

        public void Foo() { }
    }
}}