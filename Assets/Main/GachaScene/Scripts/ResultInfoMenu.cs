using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static STAT;
using static UnityEngine.Rendering.DebugUI;

public class ResultInfoMenu : MonoBehaviour
{
    [Header("References")]
    public Canvas myCanvas;
    public Vector2 followOffset;
    public EquipmentColor equipmentColor;
    public Text equipmentType;
    public Text equipmentRarity;
    public Text mainStat;
    public Text subStats;


    [HideInInspector]
    public EntityEquipment entityEquipment;
    // Start is called before the first frame update
    void Start()
    {
        
    }

   
    void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
        transform.position = myCanvas.transform.TransformPoint(pos + followOffset);
    }

    public void UpdateData(EntityEquipment entityEquipment)
    {
        this.entityEquipment = entityEquipment;

        equipmentType.text = entityEquipment.equipmentType.ToString();
        equipmentRarity.text = entityEquipment.equipmentRarity.ToString();
        equipmentRarity.color = equipmentColor.GetColorOfRarity(entityEquipment.equipmentRarity);
        mainStat.text = EquipmentStatsToString(entityEquipment.mainStat.statType, entityEquipment.mainStat.value);
        subStats.text = "";
        foreach (STAT substat in entityEquipment.subStats)
            subStats.text += EquipmentStatsToString(substat.statType, substat.value) + "\n";
    }

    /// <summary>
    /// Helper function to convert Equipment Stats To String for UI display
    /// </summary>
    /// <returns>String that was converted from the stat provided.</returns>
    string EquipmentStatsToString(STAT.STAT_TYPE statType, int value)
    {
        string finalString = "";
        switch (statType)
        {
            case STAT.STAT_TYPE.DMG_BOOST:
                finalString = "DAMAGE BOOST: ";
                break;
            case STAT.STAT_TYPE.DMG_REDUCTION:
                finalString = "DAMAGE BLOCK: ";
                break;
            case STAT.STAT_TYPE.FLIGHT_SPEED:
                finalString = "SPEED BOOST: ";
                break;
            case STAT.STAT_TYPE.LOWER_FUEL_CONSUMPTION:
                finalString = "FUEL REDUCED: ";
                break;
        }
        finalString += value.ToString() + "%";
        return finalString;
    }
}
