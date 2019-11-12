// <copyright file="ExpressionUtility.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Some of this code was taken from http://www.yoda.arachsys.com/csharp/miscutil/usage/genericoperators.html.
    /// General purpose Expression utilities.
    /// </summary>
    public static class ExpressionUtility
    {
        private static readonly Dictionary<Type, Type[]> NumericTypePromotions;

        static ExpressionUtility()
        {
            NumericTypePromotions = new Dictionary<Type, Type[]>();

            static void Add(Type key, params Type[] types) => NumericTypePromotions[key] = types;

            // Using the promotion table at
            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/conversion-tables
            Add(typeof(byte), typeof(ushort), typeof(short), typeof(uint), typeof(int), typeof(ulong), typeof(long), typeof(float), typeof(double), typeof(decimal));
            Add(typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal));
            Add(typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal));
            Add(typeof(ushort), typeof(uint), typeof(int), typeof(ulong), typeof(long), typeof(float), typeof(double), typeof(decimal));
            Add(typeof(char), typeof(ushort), typeof(uint), typeof(int), typeof(ulong), typeof(long), typeof(float), typeof(double), typeof(decimal));
            Add(typeof(int), typeof(long), typeof(double), typeof(decimal), typeof(float));
            Add(typeof(uint), typeof(long), typeof(ulong), typeof(double), typeof(decimal), typeof(float));
            Add(typeof(long), typeof(decimal), typeof(float), typeof(double));
            Add(typeof(ulong), typeof(decimal), typeof(float), typeof(double));
            Add(typeof(float), typeof(double));
            Add(typeof(decimal), typeof(float), typeof(double));
            Add(typeof(double));
        }

        /// <summary>
        /// Perform the implicit conversions as set out in the C# spec docs at
        /// https://docs.microsoft.com/en-us/dotnet/standard/base-types/conversion-tables.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static Type BinaryNumericResultType(Type left, Type right)
        {
            if (left == right)
            {
                return left;
            }

            if (!NumericTypePromotions.ContainsKey(left))
            {
                throw new System.ArgumentException("Argument is not numeric", nameof(left));
            }

            if (!NumericTypePromotions.ContainsKey(right))
            {
                throw new System.ArgumentException("Argument is not numeric", nameof(right));
            }

            // Test left to right promotion
            if (NumericTypePromotions[right].Contains(left))
            {
                return right;
            }

            if (NumericTypePromotions[left].Contains(right))
            {
                return left;
            }

            throw new Exception("Should not get here in code");
        }

        private static (Expression left, Expression right) Cast(Expression lhs, Expression rhs, Type leftType, Type rightType, Type resultType)
        {
            Expression castLhs = leftType == resultType ? lhs : (Expression)Expression.Convert(lhs, resultType);
            Expression castRhs = rightType == resultType ? rhs : (Expression)Expression.Convert(rhs, resultType);
            return (castLhs, castRhs);
        }

        /// <summary>
        /// Create a function delegate representing a binary operation.
        /// </summary>
        /// <param name="body">Body factory.</param>
        /// <param name="leftType"></param>
        /// <param name="rightType"></param>
        /// <exception cref="System.ArgumentException"></exception>
        /// <returns>Compiled function delegate.</returns>
        public static Delegate CreateExpression(
            Func<Expression, Expression, BinaryExpression> body,
            Type leftType,
            Type rightType)
        {
            ParameterExpression lhs = Expression.Parameter(leftType, "lhs");
            ParameterExpression rhs = Expression.Parameter(rightType, "rhs");
            try
            {
                try
                {
                    Type resultType = BinaryNumericResultType(leftType, rightType);
                    (Expression castLhs, Expression castRhs) = Cast(lhs, rhs, leftType, rightType, resultType);
                    return Expression.Lambda(body(castLhs, castRhs), lhs, rhs).Compile();
                }
                catch (InvalidOperationException)
                {
                    try
                    {
                        Type resultType = leftType;
                        (Expression castLhs, Expression castRhs) = Cast(lhs, rhs, leftType, rightType, resultType);
                        return Expression.Lambda(body(castLhs, castRhs), lhs, rhs).Compile();
                    }
                    catch (InvalidOperationException)
                    {
                        Type resultType = rightType;
                        (Expression castLhs, Expression castRhs) = Cast(lhs, rhs, leftType, rightType, resultType);
                        return Expression.Lambda(body(castLhs, castRhs), lhs, rhs).Compile();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message; // avoid capture of ex itself
                Action action = () => { throw new InvalidOperationException(msg); };
                return action;
            }
        }
    }
}
