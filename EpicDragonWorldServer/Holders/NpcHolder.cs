/**
 * Author: Pantelis Andrianakis
 * Date: November 28th 2019
 */
public class NpcHolder
{
    private readonly int _npcId;
    private readonly NpcType _npcType;
    private readonly int _level;
    private readonly bool _sex;
    private readonly long _hp;
    private readonly int _stamina;
    private readonly int _strength;
    private readonly int _dexterity;
    private readonly int _intelect;

    public NpcHolder(int npcId, NpcType npcType, int level, bool sex, long hp, int stamina, int strength, int dexterity, int intelect)
    {
        _npcId = npcId;
        _npcType = npcType;
        _level = level;
        _sex = sex;
        _hp = hp;
        _stamina = stamina;
        _strength = strength;
        _dexterity = dexterity;
        _intelect = intelect;
    }

    public int GetNpcId()
    {
        return _npcId;
    }

    public NpcType GetNpcType()
    {
        return _npcType;
    }

    public int GetLevel()
    {
        return _level;
    }

    public bool IsFemale()
    {
        return _sex;
    }

    public long GetHp()
    {
        return _hp;
    }

    public int GetSTA()
    {
        return _stamina;
    }

    public int GetSTR()
    {
        return _strength;
    }

    public int GetDEX()
    {
        return _dexterity;
    }

    public int GetINT()
    {
        return _intelect;
    }
}
