using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class ReinforcementsUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    ConfirmationMenuManager confirmMenu;

    [Space(10)]
    [SerializeField]
    RectTransform scoutEntityContainer;
    [SerializeField]
    RectTransform balancedEntityContainer;
    [SerializeField]
    RectTransform assaultEntityContainer;

    [SerializeField]
    TextMeshProUGUI reinforcementsText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForPlayerBeforeInit());
    }

    IEnumerator WaitForPlayerBeforeInit()
    {
        while (PlayerManager.instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        reinforcementsText.text = "Reinforcements:\n";
        if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
        {
            EntityObject scoutEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.StealthWing);
            scoutEntityContainer.Find("Icon").GetComponent<Image>().sprite = scoutEntity.entityIcon;
            scoutEntityContainer.Find("Name").GetComponent<TextMeshProUGUI>().text = scoutEntity.entityName;
            scoutEntityContainer.Find("Cost").GetComponent<TextMeshProUGUI>().text = ResourceReference.instance.currencySettings.scoutPurchaseCost.ToString() + " coins";
            EntityObject balancedEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.Whitebeard);
            balancedEntityContainer.Find("Icon").GetComponent<Image>().sprite = balancedEntity.entityIcon;
            balancedEntityContainer.Find("Name").GetComponent<TextMeshProUGUI>().text = balancedEntity.entityName;
            balancedEntityContainer.Find("Cost").GetComponent<TextMeshProUGUI>().text = ResourceReference.instance.currencySettings.balancedPurchaseCost.ToString() + " coins";
            EntityObject assaultEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.F16);
            assaultEntityContainer.Find("Icon").GetComponent<Image>().sprite = assaultEntity.entityIcon;
            assaultEntityContainer.Find("Name").GetComponent<TextMeshProUGUI>().text = assaultEntity.entityName;
            assaultEntityContainer.Find("Cost").GetComponent<TextMeshProUGUI>().text = ResourceReference.instance.currencySettings.assaultPurchaseCost.ToString() + " coins";
        }
        else if (PlayerManager.instance.playerTeam == TEAM_TYPE.INVADERS)
        {
            EntityObject scoutEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.Mako);
            scoutEntityContainer.Find("Icon").GetComponent<Image>().sprite = scoutEntity.entityIcon;
            scoutEntityContainer.Find("Name").GetComponent<TextMeshProUGUI>().text = scoutEntity.entityName;
            scoutEntityContainer.Find("Cost").GetComponent<TextMeshProUGUI>().text = ResourceReference.instance.currencySettings.scoutPurchaseCost.ToString() + " coins";
            EntityObject balancedEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.X_Wing);
            balancedEntityContainer.Find("Icon").GetComponent<Image>().sprite = balancedEntity.entityIcon;
            balancedEntityContainer.Find("Name").GetComponent<TextMeshProUGUI>().text = balancedEntity.entityName;
            balancedEntityContainer.Find("Cost").GetComponent<TextMeshProUGUI>().text = ResourceReference.instance.currencySettings.balancedPurchaseCost.ToString() + " coins";
            EntityObject assaultEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.Deathrow);
            assaultEntityContainer.Find("Icon").GetComponent<Image>().sprite = assaultEntity.entityIcon;
            assaultEntityContainer.Find("Name").GetComponent<TextMeshProUGUI>().text = assaultEntity.entityName;
            assaultEntityContainer.Find("Cost").GetComponent<TextMeshProUGUI>().text = ResourceReference.instance.currencySettings.assaultPurchaseCost.ToString() + " coins";
        }
    }

    // Update is called once per frame
    void Update()
    {
        reinforcementsText.text = "Reinforcements:\n";
        if (PlayerManager.instance != null)
        {
            if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
            {
                EntityObject scoutEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.StealthWing);
                EntityObject balancedEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.Whitebeard);
                EntityObject assaultEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.F16);
                if (GameplayManager.instance.defenderSpawnWave.ContainsKey(scoutEntity.entityType))
                    reinforcementsText.text += GameplayManager.instance.defenderSpawnWave[scoutEntity.entityType].ToString() + "x " + scoutEntity.entityName + "\n";
                else
                    reinforcementsText.text += "0x " + scoutEntity.entityName + "\n";
                if (GameplayManager.instance.defenderSpawnWave.ContainsKey(balancedEntity.entityType))
                    reinforcementsText.text += GameplayManager.instance.defenderSpawnWave[balancedEntity.entityType].ToString() + "x " + balancedEntity.entityName + "\n";
                else
                    reinforcementsText.text += "0x " + balancedEntity.entityName + "\n";
                if (GameplayManager.instance.defenderSpawnWave.ContainsKey(assaultEntity.entityType))
                    reinforcementsText.text += GameplayManager.instance.defenderSpawnWave[assaultEntity.entityType].ToString() + "x " + assaultEntity.entityName;
                else
                    reinforcementsText.text += "0x " + assaultEntity.entityName;
            }
            else
            {
                EntityObject scoutEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.Mako);
                EntityObject balancedEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.X_Wing);
                EntityObject assaultEntity = ResourceReference.instance.entityList.GetEntityObject(EntityTypes.Deathrow);
                if (GameplayManager.instance.invaderSpawnWave.ContainsKey(scoutEntity.entityType))
                    reinforcementsText.text += GameplayManager.instance.invaderSpawnWave[scoutEntity.entityType].ToString() + "x " + scoutEntity.entityName + "\n";
                else
                    reinforcementsText.text += "0x " + scoutEntity.entityName + "\n";
                if (GameplayManager.instance.invaderSpawnWave.ContainsKey(balancedEntity.entityType))
                    reinforcementsText.text += GameplayManager.instance.invaderSpawnWave[balancedEntity.entityType].ToString() + "x " + balancedEntity.entityName + "\n";
                else
                    reinforcementsText.text += "0x " + balancedEntity.entityName + "\n";
                if (GameplayManager.instance.invaderSpawnWave.ContainsKey(assaultEntity.entityType))
                    reinforcementsText.text += GameplayManager.instance.invaderSpawnWave[assaultEntity.entityType].ToString() + "x " + assaultEntity.entityName;
                else
                    reinforcementsText.text += "0x " + assaultEntity.entityName;
            }
        }
    }

    public void BuyEntity(Button selectedButton)
    {
        EntityTypes entityTypePurchase = ResourceReference.instance.entityList.GetEntityObjectByName(selectedButton.transform.Find("Name").GetComponent<TextMeshProUGUI>().text).entityType;
        confirmMenu.AskPurchase(entityTypePurchase);
    }
}
