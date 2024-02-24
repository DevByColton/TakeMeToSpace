using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TakeMeToSpace.Base.Reader;

public class TileMapReader
{
    public List<TileRow> ReadTileRows(string path)
    {
        string fileContents = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<TileRow>>(fileContents);
    }
}