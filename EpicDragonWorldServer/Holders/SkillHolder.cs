/**
 * Author: Pantelis Andrianakis
 * Date: May 5th 2019
 */
public class SkillHolder
{
    private readonly int _skillId;
    private readonly int _level;
    private readonly SkillType _skillType;
    private readonly int _reuse;
    private readonly int _range;
    private readonly int _param1;
    private readonly int _param2;

    public SkillHolder(int skillId, int level, SkillType skillType, int reuse, int range, int param1, int param2)
    {
        _skillId = skillId;
        _level = level;
        _skillType = skillType;
        _reuse = reuse;
        _range = range;
        _param1 = param1;
        _param2 = param2;
    }

    public int GetSkillId()
    {
        return _skillId;
    }

    public int GetLevel()
    {
        return _level;
    }

    public SkillType GetSkillType()
    {
        return _skillType;
    }

    public int GetReuse()
    {
        return _reuse;
    }

    public int GetRange()
    {
        return _range;
    }

    public int GetParam1()
    {
        return _param1;
    }

    public int GetParam2()
    {
        return _param2;
    }
}
