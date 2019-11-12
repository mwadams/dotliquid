// <copyright file="RenderParameters.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Rendering parameters.
    /// </summary>
    public class RenderParameters
    {
        /// <summary>
        /// Gets or sets if you provide a Context object, you do not need to set any other parameters.
        /// </summary>
        public Context Context { get; set; }

        /// <summary>
        /// Gets or sets hash of local variables used during rendering.
        /// </summary>
        public Hash LocalVariables { get; set; }

        /// <summary>
        /// Gets or sets filters used during rendering.
        /// </summary>
        public IEnumerable<Type> Filters { get; set; }

        /// <summary>
        /// Gets or sets hash of user-defined, internally-available variables.
        /// </summary>
        public Hash Registers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value that controls whether errors are thrown as exceptions.
        /// </summary>
        [Obsolete("Use ErrorsOutputMode instead")]
        public bool RethrowErrors
        {
            get { return this.ErrorsOutputMode == ErrorsOutputMode.Rethrow; }
            set { this.ErrorsOutputMode = value ? ErrorsOutputMode.Rethrow : ErrorsOutputMode.Display; }
        }

        private ErrorsOutputMode erorsOutputMode = ErrorsOutputMode.Display;

        /// <summary>
        /// Gets or sets errors output mode.
        /// </summary>
        public ErrorsOutputMode ErrorsOutputMode
        {
            get
            {
                return this.erorsOutputMode;
            }

            set
            {
                this.erorsOutputMode = value;
            }
        }

        private int maxIterations = 0;

        /// <summary>
        /// Gets or sets maximum number of iterations for the For tag.
        /// </summary>
        public int MaxIterations
        {
            get { return this.maxIterations; }
            set { this.maxIterations = value; }
        }

        private int timeout = 0;

        public IFormatProvider FormatProvider { get; }

        public RenderParameters(IFormatProvider formatProvider)
        {
            this.FormatProvider = formatProvider ?? throw new ArgumentNullException(nameof(formatProvider));
        }

        /// <summary>
        /// Gets or sets rendering timeout in ms.
        /// </summary>
        public int Timeout
        {
            get { return this.timeout; }
            set { this.timeout = value; }
        }

        internal void Evaluate(Template template, out Context context, out Hash registers, out IEnumerable<Type> filters)
        {
            if (this.Context != null)
            {
                context = this.Context;
                registers = null;
                filters = null;
                context.RestartTimeout();
                return;
            }

            var environments = new List<Hash>();
            if (this.LocalVariables != null)
            {
                environments.Add(this.LocalVariables);
            }

            if (template.IsThreadSafe)
            {
                context = new Context(environments, new Hash(), new Hash(), this.ErrorsOutputMode, this.MaxIterations, this.Timeout, this.FormatProvider);
            }
            else
            {
                environments.Add(template.Assigns);
                context = new Context(environments, template.InstanceAssigns, template.Registers, this.ErrorsOutputMode, this.MaxIterations, this.Timeout, this.FormatProvider);
            }

            registers = this.Registers;
            filters = this.Filters;
        }

        /// <summary>
        /// Creates a RenderParameters from a context.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public static RenderParameters FromContext(Context context, IFormatProvider formatProvider)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new RenderParameters(formatProvider) { Context = context };
        }
    }
}
