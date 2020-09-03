
using System;
/**
* Author: Pantelis Andrianakis
* Date: November 28th 2019
*/
public class Npc : Creature
{
    private readonly NpcHolder _npcHolder;
    private readonly SpawnHolder _spawnHolder;

    public Npc(NpcHolder npcHolder, SpawnHolder spawnHolder)
    {
        _npcHolder = npcHolder;
        _spawnHolder = spawnHolder;

        SetCurrentHp(npcHolder.GetHp());
        // TODO: Implement Creature stats holder.
        SetLocation(spawnHolder.GetLocation());
    }

    public NpcHolder GetNpcHolder()
    {
        return _npcHolder;
    }

    public SpawnHolder GetSpawnHolder()
    {
        return _spawnHolder;
    }

    public override bool IsNpc()
    {
        return true;
    }

    public override Npc AsNpc()
    {
        return this;
    }

    public override string ToString()
    {
        return "NPC [" + _npcHolder.GetNpcId() + "]";
    }
}
