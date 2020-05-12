/**
 * Author: Pantelis Andrianakis
 * Date: May 5th 2019
 */
public class ItemTemplateHolder
{
    private readonly int itemId;
    private readonly ItemSlot itemSlot;
    private readonly ItemType itemType;
    private readonly bool stackable;
    private readonly bool tradable;
    private readonly int stamina;
    private readonly int strength;
    private readonly int dexterity;
    private readonly int intelect;
    private readonly SkillHolder skillHolder;

    public ItemTemplateHolder(int itemId, ItemSlot itemSlot, ItemType itemType, bool stackable, bool tradable, int stamina, int strength, int dexterity, int intelect, SkillHolder skillHolder)
    {
        this.itemId = itemId;
        this.itemSlot = itemSlot;
        this.itemType = itemType;
        this.stackable = stackable;
        this.tradable = tradable;
        this.stamina = stamina;
        this.strength = strength;
        this.dexterity = dexterity;
        this.intelect = intelect;
        this.skillHolder = skillHolder;
    }

    public int GetItemId()
    {
        return itemId;
    }

    public ItemSlot GetItemSlot()
    {
        return itemSlot;
    }

    public ItemType GetItemType()
    {
        return itemType;
    }

    public bool IsStackable()
    {
        return stackable;
    }

    public bool IsTradable()
    {
        return tradable;
    }

    public int GetSTA()
    {
        return stamina;
    }

    public int GetSTR()
    {
        return strength;
    }

    public int GetDEX()
    {
        return dexterity;
    }

    public int GetINT()
    {
        return intelect;
    }

    public SkillHolder GetSkillHolder()
    {
        return skillHolder;
    }
}
