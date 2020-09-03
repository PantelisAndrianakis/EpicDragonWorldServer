using System.Collections.Generic;

/**
 * Author: Pantelis Andrianakis
 * Date: April 27th 2019
 */
public class RegionHolder
{
    private readonly int _x;
    private readonly int _z;
    private readonly List<WorldObject> _objects = new List<WorldObject>();
    private List<RegionHolder> _surroundingRegions;

    public RegionHolder(int x, int z)
    {
        _x = x;
        _z = z;
    }

    public void SetSurroundingRegions(List<RegionHolder> regions)
    {
        _surroundingRegions = regions;

        // Make sure that this region is always first to improve bulk operations.
        for (int i = 0; i < _surroundingRegions.Count; i++)
        {
            if (_surroundingRegions[i] == this)
            {
                RegionHolder first = _surroundingRegions[0];
                _surroundingRegions[0] = this;
                _surroundingRegions[i] = first;
                break;
            }
        }
    }

    public List<RegionHolder> GetSurroundingRegions()
    {
        return _surroundingRegions;
    }

    public void AddObject(WorldObject obj)
    {
        lock (_objects)
        {
            _objects.Remove(obj);
            _objects.Add(obj);
        }
    }

    public void RemoveObject(WorldObject obj)
    {
        lock (_objects)
        {
            _objects.Remove(obj);
        }
    }

    public List<WorldObject> GetObjects()
    {
        return _objects;
    }

    public int GetX()
    {
        return _x;
    }

    public int GetZ()
    {
        return _z;
    }

    public override int GetHashCode()
    {
        return _x ^ _z;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        RegionHolder region = ((RegionHolder)obj);
        return _x == region.GetX() && _z == region.GetZ();
    }

    public override string ToString()
    {
        return "Region [" + _x + " " + _z + "]";
    }
}
