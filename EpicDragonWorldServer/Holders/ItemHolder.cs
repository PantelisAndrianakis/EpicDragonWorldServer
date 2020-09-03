/**
 * Author: Pantelis Andrianakis
 * Date: March 12th 2020
 */
public class ItemHolder
{
    private readonly ItemTemplateHolder _itemTemplate;
    private int _quantity = 1;
    private int _enchant = 0;

    public ItemHolder(ItemTemplateHolder itemTemplate)
    {
        _itemTemplate = itemTemplate;
    }

    public ItemTemplateHolder GetTemplate()
    {
        return _itemTemplate;
    }

    public void SetQuantity(int quantity)
    {
        _quantity = quantity;
    }

    public int GetQuantity()
    {
        return _quantity;
    }

    public void SetEnchant(int enchant)
    {
        _enchant = enchant;
    }

    public int GetEnchant()
    {
        return _enchant;
    }
}
