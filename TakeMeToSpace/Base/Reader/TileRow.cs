using System.Collections.Generic;

namespace TakeMeToSpace.Base.Reader;

public class TileRow
{
    public List<TileColumn> TileColumns { get; set; } = new();
}