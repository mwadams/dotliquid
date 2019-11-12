// <copyright file="IfChanged.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.Tags
{
    using System.IO;
    using System.Threading.Tasks;

    public class IfChanged : DotLiquid.Block
    {
        public override Task RenderAsync(Context context, TextWriter result)
        {
            return context.Stack(async () =>
            {
                string tempString;
                using (TextWriter temp = new StringWriter(result.FormatProvider))
                {
                    await this.RenderAllAsync(this.NodeList, context, temp).ConfigureAwait(false);
                    tempString = temp.ToString();
                }

                if (tempString != (context.Registers["ifchanged"] as string))
                {
                    context.Registers["ifchanged"] = tempString;
                    await result.WriteAsync(tempString).ConfigureAwait(false);
                }
            });
        }
    }
}
