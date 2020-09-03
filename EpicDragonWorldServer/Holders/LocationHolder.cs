/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public struct LocationHolder
{
    float _x;
    float _y;
    float _z;
    float _heading;

    public LocationHolder(float x, float y, float z)
    {
        _x = x;
        _y = y;
        _z = z;
        _heading = 0;
    }

    public LocationHolder(float x, float y, float z, float heading)
    {
        _x = x;
        _y = y;
        _z = z;
        _heading = heading;
    }

    public float GetX()
    {
        return _x;
    }

    public void SetX(float x)
    {
        _x = x;
    }

    public float GetY()
    {
        return _y;
    }

    public void SetY(float y)
    {
        _y = y;
    }

    public float GetZ()
    {
        return _z;
    }

    public void SetZ(float z)
    {
        _z = z;
    }

    public float GetHeading()
    {
        return _heading;
    }

    public void SetHeading(float heading)
    {
        _heading = heading;
    }

    public void Update(float x, float y, float z, float heading)
    {
        _x = x;
        _y = y;
        _z = z;
        _heading = heading;
    }

    public override string ToString()
    {
        string result = _x + " " + _y + " " + _z;
        return "Location [" + (_heading > 0 ? result + " " + _heading : result) + "]";
    }
}
