/**
 * Author: Pantelis Andrianakis
 * Date: November 28th 2019
 */
public class SpawnHolder
{
    private readonly LocationHolder _location;
    private readonly int _respawnDelay;

    public SpawnHolder(LocationHolder location, int respawnDelay)
    {
        _location = location;
        _respawnDelay = respawnDelay;
    }

    public LocationHolder GetLocation()
    {
        return _location;
    }

    public int GetRespawnDelay()
    {
        return _respawnDelay;
    }
}
