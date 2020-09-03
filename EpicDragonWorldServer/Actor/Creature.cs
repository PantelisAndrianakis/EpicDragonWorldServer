/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class Creature : WorldObject
{
    private long _currentHp = 0;
    private long _currentMp = 0;
    private WorldObject _target;

    // TODO: Implement Player level data.
    // TODO: Implement Creature stats holder.
    public long GetMaxHp()
    {
        return 100;
    }

    public void SetCurrentHp(long value)
    {
        _currentHp = value;
    }

    public long GetCurrentHp()
    {
        return _currentHp;
    }

    public void SetCurrentMp(long value)
    {
        _currentMp = value;
    }

    public long GetCurrentMp()
    {
        return _currentMp;
    }

    public void SetTarget(WorldObject worldObject)
    {
        _target = worldObject;
    }

    public WorldObject GetTarget()
    {
        return _target;
    }

    public override bool IsCreature()
    {
        return true;
    }

    public override Creature AsCreature()
    {
        return this;
    }

    public override string ToString()
    {
        return "Creature [" + GetObjectId() + "]";
    }
}
