using Gaia;
using UnityEngine;

public sealed class WorldGenerator
{
    private readonly WorldDesigner worldDesigner;
    private readonly Transform runtime;

    public WorldGenerator(WorldDesigner worldDesigner, Transform runtime)
    {
        this.worldDesigner = worldDesigner;
        this.runtime = runtime;
    }


}