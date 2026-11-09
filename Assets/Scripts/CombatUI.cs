using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CombatUI : MonoBehaviour
{
    public EquipmentSystem equipmentSystem;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;

    public Button attackButton;
    public TMP_Text attackButtonText;
    public TMP_Text actionPointText;

    public Transform attackButtonContainer;
    public Button attackButtonPrefab;

    private List<Button> spawnedButtons = new List<Button>();

    void Start()
    {
        // RefreshAttackButton();
        RefreshAttackButtons();

        // attackButton.onClick.AddListener(
        //     playerCombat.SelectAttack
        // );
    }

    public void RefreshAttackButtons()
    {
        foreach (Button button in spawnedButtons)
            Destroy(button.gameObject);

        spawnedButtons.Clear();

        EquipmentItem weapon = equipmentSystem.GetEquippedItem(EquipmentSlot.MainHand);

        if (weapon == null)
            return;

        foreach (AttackOption attack in weapon.attackOptions)
        {
            Button button = Instantiate(attackButtonPrefab, attackButtonContainer);

            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
                buttonText.text = attack.attackName + " (" + attack.attackAPCost + " AP)";

            button.onClick.AddListener(() => playerCombat.SelectAttack(attack));

            spawnedButtons.Add(button);
        }
    }

    void RefreshAttackButton()
    {
        EquipmentItem weapon =
            equipmentSystem.GetEquippedItem(EquipmentSlot.MainHand);

        if (weapon == null)
        {
            attackButtonText.text = "No Weapon";
            attackButton.interactable = false;
        }
        else
        {
            //Hard coded to 1 ability
            attackButtonText.text = weapon.attackOptions[0].attackName;
            attackButton.interactable = true;
        }
    }

    // void OnDestroy()
    // {
    //     if (attackButton != null && playerCombat != null)
    //     {
    //         attackButton.onClick.RemoveListener(
    //             playerCombat.SelectAttack
    //         );
    //     }
    // }

    void Update()
    {
        actionPointText.text = "AP: " +
                    playerMovement.currentAP + " / " +
                    playerMovement.maxAP;
    }
}