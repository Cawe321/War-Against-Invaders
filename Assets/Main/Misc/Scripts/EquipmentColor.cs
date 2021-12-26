using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentColor", menuName = "WarAgainstInvaders/Settings/EquipmentColor", order = 3)]
public class EquipmentColor : ScriptableObject
{
    public Color commonColor;
    public Color rareColor;
    public Color legendaryColor;

    public Color GetColorOfRarity(EntityEquipment.EQUIPMENT_RARITY equipmentRarity)
    {
        switch (equipmentRarity)
        {
            case EntityEquipment.EQUIPMENT_RARITY.COMMON:
                return commonColor;
            case EntityEquipment.EQUIPMENT_RARITY.RARE:
                return rareColor;
            case EntityEquipment.EQUIPMENT_RARITY.LEGENDARY:
                return legendaryColor;
        }
        return Color.black;
    }
}
