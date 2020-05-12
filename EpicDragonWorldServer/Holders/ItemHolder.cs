/**
 * Author: Pantelis Andrianakis
 * Date: March 12th 2020
 */
public class ItemHolder
{
    private readonly ItemTemplateHolder itemTemplate;
    private int quantity = 1;
    private int enchant = 0;

    public ItemHolder(ItemTemplateHolder itemTemplate)
    {
        this.itemTemplate = itemTemplate;
    }

    public ItemTemplateHolder GetTemplate()
    {
        return itemTemplate;
    }

    public void SetQuantity(int quantity)
    {
        this.quantity = quantity;
    }

    public int GetQuantity()
    {
        return quantity;
    }

    public void SetEnchant(int enchant)
    {
        this.enchant = enchant;
    }

    public int GetEnchant()
    {
        return enchant;
    }
}
