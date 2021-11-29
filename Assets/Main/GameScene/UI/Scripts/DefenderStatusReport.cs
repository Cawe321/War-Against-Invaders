using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenderStatusReport : MonoBehaviour
{

    [Header("Global References")]
    [SerializeField]
    EntityHealth[] warehouses;

    [Header("UI References")]
    [SerializeField]
    ConfirmationMenuManager confirmMenu;

    [SerializeField]
    TextMeshProUGUI spaceshipETA;
    [SerializeField]
    TextMeshProUGUI[] warehouseHealthText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float estimatedSeconds = ((GameplayManager.instance.spaceshipEntity.transform.position - new Vector3(GameplayManager.instance.dockEntity.transform.position.x, GameplayManager.instance.spaceshipEntity.transform.position.y, GameplayManager.instance.dockEntity.transform.position.z)).magnitude)/ GameplayManager.instance.spaceshipEntity.moveSpeed / Time.fixedDeltaTime * 0.001f;
        spaceshipETA.text = "Enemy Spaceship E.T.A:\n<i>" + (int)estimatedSeconds + " seconds</i>";

        for (int i = 0; i < warehouses.Length; ++i)
            warehouseHealthText[i].text = "Health:\n" + (int)warehouses[i].currHealth + "/" + (int)warehouses[i].maxHealth;
    }

    public void AskRepair(Button selectedButton)
    {

        for (int i = 0; i < warehouseHealthText.Length; ++i)
        {
            if (selectedButton.transform == warehouseHealthText[i].transform.parent)
            {
                if ((int)warehouses[i].currHealth == (int)warehouses[i].maxHealth)
                    confirmMenu.AskRepair(warehouses[i], "Warehouse", ResourceReference.instance.currencySettings.warehouseRepairCost);
                return;
            }
        }
    }
}
