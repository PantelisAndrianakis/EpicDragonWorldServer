using System.Collections.Generic;

/**
 * Author: Pantelis Andrianakis
 * Date: April 27th 2019
 */
public class RegionHolder
{
    private readonly int x;
    private readonly int z;
    private readonly List<WorldObject> objects = new List<WorldObject>();
    private List<RegionHolder> surroundingRegions;

    public RegionHolder(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public void SetSurroundingRegions(List<RegionHolder> regions)
    {
        surroundingRegions = regions;

        // Make sure that this region is always first to improve bulk operations.
        for (int i = 0; i < surroundingRegions.Count; i++)
        {
            if (surroundingRegions[i] == this)
            {
                RegionHolder first = surroundingRegions[0];
                surroundingRegions[0] = this;
                surroundingRegions[i] = first;
                break;
            }
        }
    }

    public List<RegionHolder> GetSurroundingRegions()
    {
        return surroundingRegions;
    }

    public void AddObject(WorldObject obj)
    {
        lock (objects)
        {
            objects.Remove(obj);
            objects.Add(obj);
        }
    }

    public void RemoveObject(WorldObject obj)
    {
        lock (objects)
        {
            objects.Remove(obj);
        }
    }

    public List<WorldObject> GetObjects()
    {
        return objects;
    }

    public int GetX()
    {
        return x;
    }

    public int GetZ()
    {
        return z;
    }

    public override int GetHashCode()
    {
        return x ^ z;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        RegionHolder region = ((RegionHolder)obj);
        return x == region.GetX() && z == region.GetZ();
    }

    public override string ToString()
    {
        return "Region [" + x + " " + z + "]";
    }
}
