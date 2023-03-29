using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CODE_Interpreter.Content;

namespace CODE_Interpreter
{
    internal class Operators : CodeBaseVisitor<object>
    {
        public static object? Add(object? left, object? right)
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

        public static object? Subtract(object? left, object? right)
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

        public static object? Multiply(object? left, object? right)
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

        public static object? Divide(object? left, object? right)
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

        public static object? Modulo(object? left, object? right)
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

        public static object? GreaterThan(object? left, object? right)
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

        public static object? LesserThan(object? left, object? right)
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

        public static object? GreaterThanEqual(object? left, object? right)
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

        public static object? LesserThanEqual(object? left, object? right)
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

        public static object? Equal(object? left, object? right)
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

        public static object? NotEqual(object? left, object? right)
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

        public static object? AndBoolean(object? left, object? right)
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

        public static object? OrBoolean(object? left, object? right)
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

        public static object? NotBoolean(object? left)
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
    }
}
