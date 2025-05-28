using ASUI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMainTitle : WidgetsBase
{
    public override bool IsVisible => this.GameObject.activeInHierarchy;

    public TextMeshPro TexTitle;

    public override void OnInit()
    {
        this.TexTitle = this.StyleState?.GetComponentByUIName<TextMeshPro>("Tex_Title");
    }
}
