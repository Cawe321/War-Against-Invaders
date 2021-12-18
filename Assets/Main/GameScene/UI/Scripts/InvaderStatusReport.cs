using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvaderStatusReport : MonoBehaviour
{
    [Header("Global References")]
    [SerializeField] 
    EntityHealth[] fuelTanks;
    [SerializeField]
    EntityHealth[] thrusters;

    [Header("UI References")]
    [SerializeField]
    ConfirmationMenuManager confirmMenu;

    [SerializeField]
    TextMeshProUGUI stabilityText;
    [SerializeField]
    TextMeshProUGUI[] fuelTanksHealthText;
    [SerializeField]
    TextMeshProUGUI[] thrustersHealthText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stabilityText.text = "Current Stability: " + (int)(GameplayManager.instance.spaceshipEntity.baseEntity.GetCurrentStability() * 100f) + "\nMinimum Stability: " + (int)GameplayManager.instance.spaceshipEntity.baseEntity.requiredStability;

        for (int i = 0; i < fuelTanks.Length; ++i)
            fuelTanksHealthText[i].text = "Health:\n" + (int)fuelTanks[i].currHealth + "/" + (int)fuelTanks[i].maxHealth;

        for (int i = 0; i < thrusters.Length; ++i)
            thrustersHealthText[i].text = "Health:\n" + (int)thrusters[i].currHealth + "/" + (int)thrusters[i].maxHealth;
    }

    public void AskRepair(Button selectedButton)
    {
        {
            for (int i = 0; i < fuelTanksHealthText.Length; ++i)
            {
                if (selectedButton.transform == fuelTanksHealthText[i].transform.parent)
                {
                    confirmMenu.AskRepair(fuelTanks[i], "Fuel Tank" , ResourceReference.instance.currencySettings.fuelTankRepairCost);
                    return;
                }
            }
        }
        {
            for (int i = 0; i < thrustersHealthText.Length; ++i)
            {
                if (selectedButton.transform == thrustersHealthText[i].transform.parent)
                {
                    confirmMenu.AskRepair(thrusters[i], "Thruster", ResourceReference.instance.currencySettings.thrusterRepairCost);
                    return;
                }
            }
        }
    }
}
