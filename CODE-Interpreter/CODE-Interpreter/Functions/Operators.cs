﻿using CODE_Interpreter.Content;

namespace CODE_Interpreter.Functions
{
    public class Operators : CodeBaseVisitor<object>
    {
        public static object Add(object? left, object? right)
        {
            if (left is int && right is int)
            {
                return (int)left + (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left + (float)right;
            }
            else if (left is string && right is string)
            {
                return (string)left + (string)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object Subtract(object? left, object? right)
        {
            if (left is int && right is int)
            {
                return (int)left - (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left - (float)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object Multiply(object? left, object? right)
        {
            if (left is int && right is int)
            {
                return (int)left * (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left * (float)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object Divide(object? left, object? right)
        {
            if (left is int && right is int)
            {
                return (int)left / (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left / (float)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object Modulo(object? left, object? right)
        {
            if (left is int && right is int)
            {
                return (int)left % (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left % (float)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object GreaterThan(object? left, object? right)
        {
            if (left is int && right is int)
            {
                return (int)left > (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left > (float)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object LesserThan(object? left, object? right)
        {
            if (left is int && right is int)
            {
                return (int)left < (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left < (float)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object GreaterThanEqual(object? left, object? right)
        {
            if (left is int && right is int)
            {
                return (int)left >= (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left >= (float)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object LesserThanEqual(object? left, object? right)
        {
            if (left is int && right is int)
            {
                return (int)left <= (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left <= (float)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object Equal(object? left, object? right)
        {
            if (left is int && right is int)
            {
                return (int)left == (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left == (float)right;
            }
            else if (left is bool && right is bool)
            {
                return (bool)left == (bool)right;
            }
            else if (left is char && right is char)
            {
                return (char)left == (char)right;
            }
            else if (left is string && right is string)
            {
                return (string)left == (string)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object NotEqual(object? left, object? right)
        {
            if (left is int && right is int)
            {
                return (int)left != (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left != (float)right;
            }
            else if (left is bool && right is bool)
            {
                return (bool)left != (bool)right;
            }
            else if (left is char && right is char)
            {
                return (char)left != (char)right;
            }
            else if (left is string && right is string)
            {
                return (string)left != (string)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object AndBoolean(object? left, object? right)
        {
            if (left is bool && right is bool)
            {
                return (bool)left && (bool)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object OrBoolean(object? left, object? right)
        {
            if (left is bool && right is bool)
            {
                return (bool)left || (bool)right;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object NotBoolean(object? left)
        {
            if (left is bool)
            {
                return !(bool)left;
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }

        public static object Unary(string symbol, object value)
        {
            if (symbol == "+")
                return value;

            if (symbol == "-")
            {
                if (value is int i)
                    return -i;
                if (value is float f)
                    return -f;
                throw new Exception($"Cannot get unary value for symbol {symbol}");
            }

            throw new Exception($"Cannot get unary value for symbol {symbol}");
        }
        public object NewlineSymbol(string input)
        {
            return input.Replace("$", Environment.NewLine);
        }

        public object LeftParenthesis(string input)
        {
            return input.Replace("(", "");
        }

        public object RightParenthesis(string input)
        {
            return input.Replace(")", "");
        }
    }
}
