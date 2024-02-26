using System.Collections.Generic;

namespace TakeMeToSpace.Engine.Components;

public class TileRow
{
    public List<TileColumn> TileColumns { get; set; } = new();
}