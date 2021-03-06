﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public int upgradeNumb;
    private Button button;
    private TextMeshProUGUI upgradeCostDisplay;
    private Tower tower;
    private Tower upgradedTower;
    private int upgradeCost;
    void Awake()
    {
        button = GetComponent<Button>();
        upgradeCostDisplay = transform.Find("Upgrade Cost Text").gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update() {
        if (Selector.selectedObject){
            tower = Selector.selectedObject.GetComponent<TowerAI>().tower;
            if (tower.upgrade.Length > upgradeNumb){
                upgradedTower = tower.upgrade[upgradeNumb];
                upgradeCost = upgradedTower.price - tower.price;
                upgradeCostDisplay.SetText(upgradeCost.ToString());
            } else {
                if (!LeanTween.isTweening(gameObject))
                    LeanTween.scale(GetComponent<RectTransform>(), new Vector2(0,0), 0.2f).setOnComplete(ActuallyDisable);
            }

            if (upgradeCost <= MoneyManager.money){
                button.interactable = true;
            } else {
                button.interactable = false;
            }
        }
    }
    public void UpgradeTower(){
        if (upgradeCost <= MoneyManager.money && tower.upgrade.Length > upgradeNumb){
            Transform selectedTower = Selector.selectedObject.transform;
            GameObject _createdTower = Instantiate(upgradedTower.towerObj, selectedTower.position, selectedTower.rotation, selectedTower.parent);
            _createdTower.GetComponent<TowerAI>().hp = selectedTower.gameObject.GetComponent<TowerAI>().hp + (upgradedTower.maxHp-tower.maxHp);
            Destroy(selectedTower.gameObject);
            MoneyManager.GainMoney(-upgradeCost);
            Selector.SelectTower(_createdTower);
        }
    }

    public void CreateTooltip(){
        TowerAI _TowerAI = Selector.selectedObject.GetComponent<TowerAI>();
        Tooltip.CreateTowerTooltip_Static(upgradedTower, _TowerAI.attackDmgMultiplier, _TowerAI.attackRateMultiplier);
        Transform _range = Selector.selectedObject.transform.Find("Range");
        _range.localScale = Vector2.one*upgradedTower.range*2*_TowerAI.rangeMultiplier;
    }
    public void ResetRange(){
        if (Selector.selectedObject){
            Transform _range = Selector.selectedObject.transform.Find("Range");
            _range.localScale = Vector2.one*tower.range*2*Selector.selectedObject.GetComponent<TowerAI>().rangeMultiplier;
        } 
    }
    private void ActuallyDisable(){
        Tooltip.HideTooltip_Static();
        if (tower.upgrade.Length > upgradeNumb){
            gameObject.SetActive(true);
        } else {
            gameObject.SetActive(false);
        }
    }
    public void OnEnable() {
        tower = Selector.selectedObject.GetComponent<TowerAI>().tower;
        if (tower.upgrade.Length <= upgradeNumb){
            gameObject.SetActive(false);
        }
    }
}
