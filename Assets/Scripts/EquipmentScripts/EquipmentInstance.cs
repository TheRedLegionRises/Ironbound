
using System;

[Serializable]
public class EquipmentInstance
{
    public EquipmentItem item;

    public EquipmentInstance(EquipmentItem equipment)
    {
        item = equipment;
    }
}
