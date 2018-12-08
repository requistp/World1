using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;

[GameSystem]
public class TerrainSystem : GameSystem
{
    public static bool IsPassableTerrain(LocationDataInt location)
    {
        var enm = Program.slashGame.EntityManager;

        foreach (var x in enm.EntitiesWithComponent(typeof(TerrainComponent)))
        {
            if (enm.TryGetComponent(x, out FormComponent form))
            {
                if (form.Location.GetAsInt == location && enm.TryGetComponent(x, out TerrainComponent terrain))
                {
                    return !IsSolid(terrain.TerrainType);
                }
            }
        }
        return false;
    }
    public static bool IsSolid(TerrainTypeEnum terrainType)
    {
        switch (terrainType)
        {
            case TerrainTypeEnum.Dirt:
                return false;
            case TerrainTypeEnum.Rock:
                return true;
            default:
                return false;
        }
    }
    public static string Symbol(TerrainTypeEnum terrainType)
    {
        switch (terrainType)
        {
            case TerrainTypeEnum.Dirt:
                return ".";
            case TerrainTypeEnum.Rock:
                return "R";
            default:
                return " ";
        }
    }
    public static bool TryGet_Terrain(LocationDataInt location, out TerrainComponent terrain, out FormComponent form)
    {
        var enm = Program.slashGame.EntityManager;

        foreach (var x in enm.EntitiesWithComponent(typeof(TerrainComponent)))
        {
            if (enm.TryGetComponent(x, out form))
            {
                if (form.Location.GetAsInt == location)
                {
                    terrain = enm.GetComponent<TerrainComponent>(x);
                    return true;
                }
            }
        }
        terrain = null;
        form = null;
        return false;
    }

    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);
    }
}

public class TerrainComponent : IEntityComponent
{
    public int EntityID { get; }
    public TerrainTypeEnum TerrainType { get; }

    public void InitComponent(IAttributeTable attributeTable) { }

    public TerrainComponent(int entityID, TerrainTypeEnum terrainType)
    {
        EntityID = entityID;
        TerrainType = terrainType;
    }
}

public enum TerrainTypeEnum
{
    Dirt,
    Rock
}


