/**
 * Author: Pantelis Andrianakis
 * Date: May 5th 2019
 */
public class ItemTemplateHolder
{
    private readonly int _itemId;
    private readonly ItemSlot _itemSlot;
    private readonly ItemType _itemType;
    private readonly bool _stackable;
    private readonly bool _tradable;
    private readonly int _stamina;
    private readonly int _strength;
    private readonly int _dexterity;
    private readonly int _intelect;
    private readonly SkillHolder _skillHolder;

    public ItemTemplateHolder(int itemId, ItemSlot itemSlot, ItemType itemType, bool stackable, bool tradable, int stamina, int strength, int dexterity, int intelect, SkillHolder skillHolder)
    {
        _itemId = itemId;
        _itemSlot = itemSlot;
        _itemType = itemType;
        _stackable = stackable;
        _tradable = tradable;
        _stamina = stamina;
        _strength = strength;
        _dexterity = dexterity;
        _intelect = intelect;
        _skillHolder = skillHolder;
    }

    public int GetItemId()
    {
        return _itemId;
    }

    public ItemSlot GetItemSlot()
    {
        return _itemSlot;
    }

    public ItemType GetItemType()
    {
        return _itemType;
    }

    public bool IsStackable()
    {
        return _stackable;
    }

    public bool IsTradable()
    {
        return _tradable;
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

    public SkillHolder GetSkillHolder()
    {
        return _skillHolder;
    }
}
