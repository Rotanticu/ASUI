using ASUI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;
using System;

public class UIMainTitle : WidgetsBase
{
    public override bool IsVisible => this.GameObject.activeInHierarchy;

    public ASTextMeshProUGUI TexTitle;

    public override void OnInit()
    {
        this.TexTitle = this.StyleState?.GetComponentByUIName<ASTextMeshProUGUI>("tex_title");
    }
}
