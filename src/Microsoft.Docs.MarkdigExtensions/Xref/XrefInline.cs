// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Markdig.Syntax.Inlines;

namespace Microsoft.Docs.MarkdigExtensions;

public class XrefInline : LeafInline
{
    public string Href { get; set; }
}
