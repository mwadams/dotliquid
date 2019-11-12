// <copyright file="Symbol.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Util
{
    using System;

    internal class Symbol
    {
        public Func<object, bool> EvaluationFunction { get; set; }

        public Symbol(Func<object, bool> evaluationFunction)
        {
            this.EvaluationFunction = evaluationFunction;
        }
    }
}
