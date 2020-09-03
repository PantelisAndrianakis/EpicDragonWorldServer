using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class WorldObject
{
    private readonly long _objectId = IdManager.GetNextId();
    private readonly DateTime _spawnTime = DateTime.Now;
    private AnimationHolder _animations = null;
    private LocationHolder _location = new LocationHolder(0, -1000, 0);
    private RegionHolder _region = null;
    private bool _isTeleporting = false;

    public long GetObjectId()
    {
        return _objectId;
    }

    public DateTime GetSpawnTime()
    {
        return _spawnTime;
    }

    public AnimationHolder GetAnimations()
    {
        return _animations;
    }

    public void SetAnimations(AnimationHolder animations)
    {
        _animations = animations;
    }

    public LocationHolder GetLocation()
    {
        return _location;
    }

    public void SetLocation(LocationHolder location)
    {
        SetLocation(location.GetX(), location.GetY(), location.GetZ(), location.GetHeading());
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void SetLocation(float posX, float posY, float posZ, float heading)
    {
        _location.Update(posX, posY, posZ, heading);

        // When changing location test for appropriate region.
        RegionHolder testRegion = WorldManager.GetRegion(this);
        if (!testRegion.Equals(_region))
        {
            if (_region != null)
            {
                // Remove this object from the region.
                _region.RemoveObject(this);

                // Broadcast change to players left behind when teleporting.
                if (_isTeleporting)
                {
                    DeleteObject deleteObject = new DeleteObject(this);
                    List<RegionHolder> regions = _region.GetSurroundingRegions();
                    for (int i = 0; i < regions.Count; i++)
                    {
                        List<WorldObject> objects = regions[i].GetObjects();
                        for (int j = 0; j < objects.Count; j++)
                        {
                            WorldObject nearby = objects[j];
                            if (nearby == this)
                            {
                                continue;
                            }
                            if (nearby.IsPlayer())
                            {
                                nearby.AsPlayer().ChannelSend(deleteObject);
                            }
                        }
                    }
                }
            }

            // Assign new region.
            _region = testRegion;
            _region.AddObject(this);

            // Send visible NPC information.
            // TODO: Exclude known NPCs?
            if (IsPlayer())
            {
                List<WorldObject> objects = WorldManager.GetVisibleObjects(this);
                for (int i = 0; i < objects.Count; i++)
                {
                    WorldObject nearby = objects[i];
                    if (!nearby.IsNpc())
                    {
                        continue;
                    }
                    AsPlayer().ChannelSend(new NpcInformation(nearby.AsNpc()));
                }
            }
        }
    }

    public RegionHolder GetRegion()
    {
        return _region;
    }

    public void SetTeleporting()
    {
        _isTeleporting = true;
        Task.Delay(1000).ContinueWith(task => StopTeleporting());
    }

    private void StopTeleporting()
    {
        _isTeleporting = false;

        // Broadcast location to nearby players after teleporting.
        LocationUpdate locationUpdate = new LocationUpdate(this);
        List<Player> players = WorldManager.GetVisiblePlayers(this);
        Player player = AsPlayer();
        for (int i = 0; i < players.Count; i++)
        {
            Player nearby = players[i];
            if (nearby.IsPlayer())
            {
                nearby.AsPlayer().ChannelSend(locationUpdate);
            }
            if (IsPlayer())
            {
                player.ChannelSend(new LocationUpdate(nearby));
            }
        }
    }

    public bool IsTeleporting()
    {
        return _isTeleporting;
    }

    public void DeleteMe()
    {
        // Remove from region.
        _region.RemoveObject(this);

        // Broadcast NPC deletion.
        DeleteObject delete = new DeleteObject(this);
        List<RegionHolder> regions = _region.GetSurroundingRegions();
        for (int i = 0; i < regions.Count; i++)
        {
            List<WorldObject> objects = regions[i].GetObjects();
            for (int j = 0; j < objects.Count; j++)
            {
                WorldObject nearby = objects[j];
                if (nearby != null && nearby.IsPlayer())
                {
                    nearby.AsPlayer().ChannelSend(delete);
                }
            }
        }

        // Set region to null.
        _region = null;
    }

    /// <summary>Calculates distance between this WorldObject and given x, y , z.</summary>
    /// <param name="x">the X coordinate</param>
    /// <param name="y">the Y coordinate</param>
    /// <param name="z">the Z coordinate</param>
    /// <returns>distance between object and given x, y, z.</returns>
    public double CalculateDistance(float x, float y, float z)
    {
        return Math.Pow(x - _location.GetX(), 2) + Math.Pow(y - _location.GetY(), 2) + Math.Pow(z - _location.GetZ(), 2);
    }

    /// <summary>Calculates distance between this WorldObject and another WorldObject.</summary>
    /// <param name="obj">the other WorldObject</param>
    /// <returns>distance between object and given x, y, z.</returns>
    public double CalculateDistance(WorldObject obj)
    {
        return CalculateDistance(obj.GetLocation().GetX(), obj.GetLocation().GetY(), obj.GetLocation().GetZ());
    }

    /// <summary>Verify if object is instance of Creature.</summary>
    /// <returns>if object is instance of Creature.</returns>
    public virtual bool IsCreature()
    {
        return false;
    }

    /// <returns>Creature instance if current object is such, null otherwise.</returns>
    public virtual Creature AsCreature()
    {
        return null;
    }

    /// <summary>Verify if object is instance of Player.</summary>
    /// <returns>if object is instance of Player.</returns>
    public virtual bool IsPlayer()
    {
        return false;
    }

    /// <returns>Player instance if current object is such, null otherwise.</returns>
    public virtual Player AsPlayer()
    {
        return null;
    }

    /// <summary>Verify if object is instance of Npc.</summary>
    /// <returns>if object is instance of Npc.</returns>
    public virtual bool IsNpc()
    {
        return false;
    }

    /// <returns>Npc instance if current object is such, null otherwise.</returns>
    public virtual Npc AsNpc()
    {
        return null;
    }

    /// <summary>Verify if object is instance of Monster.</summary>
    /// <returns>if object is instance of Monster.</returns>
    public virtual bool IsMonster()
    {
        return false;
    }

    /// <returns>Monster instance if current object is such, null otherwise.</returns>
    public virtual Monster AsMonster()
    {
        return null;
    }

    public override string ToString()
    {
        return "WorldObject [" + _objectId + "]";
    }
}
