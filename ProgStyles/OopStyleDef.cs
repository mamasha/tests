using System;
using System.Collections.Generic;

namespace ProgStyles {
namespace Oop 
{
    abstract class Token
    {
        public abstract bool IsValue { get; }
        public abstract decimal Value { get; set; }
    }

    class ValueToken : Token
    {
        public ValueToken(decimal value)
        {
            Value = value;
        }

        public override bool IsValue
        {
            get { return true; }
        }

        public override decimal Value { get; set; }
    }

    abstract class OperatorToken : Token
    {
        public override bool IsValue
        {
            get { return false; }
        }

        public override decimal Value { get; set; }

        public abstract decimal Calculate(decimal left, decimal right);
    }

    class OperatorPlus : OperatorToken
    {
        public override decimal Calculate(decimal left, decimal right)
        {
            return left + right;
        }
    }

    class OperatorMinus : OperatorToken
    {
        public override decimal Calculate(decimal left, decimal right)
        {
            return left - right;
        }
    }

    class OperatorMultiply : OperatorToken
    {
        public override decimal Calculate(decimal left, decimal right)
        {
            return left * right;
        }
    }

    class OperatorDevide : OperatorToken
    {
        public override decimal Calculate(decimal left, decimal right)
        {
            if (right == 0)
                throw new InvalidOperationException();

            return left / right;
        }
    }
}}