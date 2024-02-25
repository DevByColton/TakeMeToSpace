using System.Collections.Generic;

namespace TakeMeToSpace.Base.Mapper;

public class TileRow
{
    public List<TileColumn> TileColumns { get; set; } = new();
}