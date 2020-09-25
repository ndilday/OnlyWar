using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleViewSoldier : MonoBehaviour
{
    public Image Soldier;
    public Image Halo;

    public void SetColor(Color color)
    {
        Soldier.color = color;
    }
}
