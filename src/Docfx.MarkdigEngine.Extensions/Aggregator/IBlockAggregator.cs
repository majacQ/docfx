// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Docfx.MarkdigEngine.Extensions;

public interface IBlockAggregator
{
    bool Aggregate(BlockAggregateContext context);
}
