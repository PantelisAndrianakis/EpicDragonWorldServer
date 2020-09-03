/**
 * Author: Pantelis Andrianakis
 * Date: December 28th 2017
 */
public class CharacterDataHolder
{
    private string _name = "";
    private byte _slot = 0;
    private bool _selected = false;
    private byte _race = 0;
    private float _height = 0.5f;
    private float _belly = 0.5f;
    private int _hairType = 0;
    private int _hairColor = 2695723;
    private int _skinColor = 15847869;
    private int _eyeColor = 2695723;
    private int _headItem = 0;
    private int _chestItem = 0;
    private int _handsItem = 0;
    private int _legsItem = 0;
    private int _feetItem = 0;
    private int _rightHandItem = 0;
    private int _leftHandItem = 0;
    private float _x = 0;
    private float _y = 0;
    private float _z = 0;
    private float _heading = 0;
    private long _experience = 0;
    private long _hp = 0;
    private long _mp = 0;
    private byte _accessLevel = 0;

    public string GetName()
    {
        return _name;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public byte GetSlot()
    {
        return _slot;
    }

    public void SetSlot(byte slot)
    {
        _slot = slot;
    }

    public bool IsSelected()
    {
        return _selected;
    }

    public void SetSelected(bool selected)
    {
        _selected = selected;
    }

    public byte GetRace()
    {
        return _race;
    }

    public void SetRace(byte race)
    {
        _race = race;
    }

    public float GetHeight()
    {
        return _height;
    }

    public void SetHeight(float height)
    {
        _height = height;
    }

    public float GetBelly()
    {
        return _belly;
    }

    public void SetBelly(float belly)
    {
        _belly = belly;
    }

    public int GetHairType()
    {
        return _hairType;
    }

    public void SetHairType(int hairType)
    {
        _hairType = hairType;
    }

    public int GetHairColor()
    {
        return _hairColor;
    }

    public void SetHairColor(int hairColor)
    {
        _hairColor = hairColor;
    }

    public int GetSkinColor()
    {
        return _skinColor;
    }

    public void SetSkinColor(int skinColor)
    {
        _skinColor = skinColor;
    }

    public int GetEyeColor()
    {
        return _eyeColor;
    }

    public void SetEyeColor(int eyeColor)
    {
        _eyeColor = eyeColor;
    }

    public int GetHeadItem()
    {
        return _headItem;
    }

    public void SetHeadItem(int headItem)
    {
        _headItem = headItem;
    }

    public int GetChestItem()
    {
        return _chestItem;
    }

    public void SetChestItem(int chestItem)
    {
        _chestItem = chestItem;
    }

    public int GetHandsItem()
    {
        return _handsItem;
    }

    public void SetHandsItem(int handsItem)
    {
        _handsItem = handsItem;
    }

    public int GetLegsItem()
    {
        return _legsItem;
    }

    public void SetLegsItem(int legsItem)
    {
        _legsItem = legsItem;
    }

    public int GetFeetItem()
    {
        return _feetItem;
    }

    public void SetFeetItem(int feetItem)
    {
        _feetItem = feetItem;
    }

    public int GetRightHandItem()
    {
        return _rightHandItem;
    }

    public void SetRightHandItem(int rightHandItem)
    {
        _rightHandItem = rightHandItem;
    }

    public int GetLeftHandItem()
    {
        return _leftHandItem;
    }

    public void SetLeftHandItem(int leftHandItem)
    {
        _leftHandItem = leftHandItem;
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

    public long GetExperience()
    {
        return _experience;
    }

    public void SetExperience(long experience)
    {
        _experience = experience;
    }

    public long GetHp()
    {
        return _hp;
    }

    public void SetHp(long hp)
    {
        _hp = hp;
    }

    public long GetMp()
    {
        return _mp;
    }

    public void SetMp(long mp)
    {
        _mp = mp;
    }

    public byte GetAccessLevel()
    {
        return _accessLevel;
    }

    public void SetAccessLevel(byte accessLevel)
    {
        _accessLevel = accessLevel;
    }
}
