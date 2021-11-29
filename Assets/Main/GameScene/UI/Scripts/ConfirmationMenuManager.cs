using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationMenuManager : MonoBehaviour
{
    [Header("Misc References")]
    [SerializeField]
    GameObject background;

    [Header("Button Confirmation Menu")]
    [SerializeField] 
    RectTransform buttonConfirmationMenu;
    [SerializeField]
    TextMeshProUGUI buttonConfirmationText;

    [Header("Slider Confirmation Menu")]
    [SerializeField]
    RectTransform sliderConfirmationMenu;
    [SerializeField]
    TextMeshProUGUI sliderConfirmationText;
    [SerializeField]
    Slider sliderConfirmationSlider;
    [SerializeField]
    Text sliderConfirmationSliderText;

    [Header("Notice Menu")]
    [SerializeField] 
    RectTransform noticeMenu;
    [SerializeField]
    TextMeshProUGUI noticeText;

    // Slider value
    float avgHealthPerCost;
    EntityHealth repairEntity;

    // Button value
    EntityTypes purchaseEntity;

    public void AskRepair(EntityHealth entity, string entityName, float fullRepairCost)
    {
        buttonConfirmationMenu.gameObject.SetActive(false);

        float totalRepairCost = (1f - (entity.currHealth / entity.maxHealth)) * fullRepairCost;
        avgHealthPerCost = (entity.maxHealth / fullRepairCost);
        repairEntity = entity;

        if (totalRepairCost > PlayerManager.instance.coins)
            totalRepairCost = PlayerManager.instance.coins;
        sliderConfirmationText.text = "Repair " + entityName + " by:";
        sliderConfirmationSlider.value = sliderConfirmationSlider.maxValue = totalRepairCost;

        sliderConfirmationMenu.gameObject.SetActive(true);
        UpdateSlider();
    }

    public void UpdateSlider()
    {
        sliderConfirmationSliderText.text = "Cost:\n" + (int)sliderConfirmationSlider.value + "\nRepair:\n" + (int)(sliderConfirmationSlider.value * avgHealthPerCost);
    }

    public void SliderConfirmationButton(bool yes)
    {
        if (yes)
            UpgradeManager.instance.RepairEntity(repairEntity, (int)sliderConfirmationSlider.value, (int)(sliderConfirmationSlider.value * avgHealthPerCost));

        sliderConfirmationMenu.gameObject.SetActive(false);
        buttonConfirmationMenu.gameObject.SetActive(false);
    }

    public void AskPurchase(EntityTypes entityType)
    {
        if (UpgradeManager.instance.CheckPlanePurchaseCost(entityType))
        {
            purchaseEntity = entityType;
            buttonConfirmationText.text = "Are you sure you want to buy " + ResourceReference.instance.entityList.GetEntityObject(entityType).entityName + " for " + UpgradeManager.instance.GetPlanePurchaseCost(entityType) + " coins?";
            buttonConfirmationMenu.gameObject.SetActive(true);
        }
        else
        {
            noticeText.text = "You don't have enough coins!";
            noticeMenu.gameObject.SetActive(true);
        }
    }

    public void ButtonConfirmationButton(bool yes)
    {
        if (yes)
            UpgradeManager.instance.PurchasePlaneEntity(purchaseEntity);
        buttonConfirmationMenu.gameObject.SetActive(false);
    }


    public void NoticeButton()
    {
        noticeMenu.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        background.SetActive(sliderConfirmationMenu.gameObject.activeInHierarchy || buttonConfirmationMenu.gameObject.activeInHierarchy || noticeMenu.gameObject.activeInHierarchy);
    }
}
