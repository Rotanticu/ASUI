using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;

namespace ASUI
{
    [Serializable]
    public class TestMainWindow : ShowHideWidget
    {
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI HeadText;
        
        public Button Button { get; private set; }
        
        [ContextMenu("OnInit")]
        public override void OnInit()
        {
            this.StyleState = this.Transform.GetComponent<ASUIStyleState>();
            this.canvasGroup = this.StyleState.GetComponentByUIName<CanvasGroup>("Canvas");
            this.HeadText = this.StyleState.GetComponentByUIName<TextMeshProUGUI>("HeadText");
            this.Button = this.StyleState.GetComponentByUIName<Button>("Button");
            this.Button.onClick.AddListener(this.RefreshTime);
        }

        public void RefreshTime()
        {
            this.HeadText.text = $"当前的时间是{DateTime.Now.ToString("HH:mm:ss")}";
        }

        protected override void OnShow()
        {
            this.HeadText.text = $"当前的时间是{DateTime.Now.ToString("HH:mm:ss")}";
        }
        
        protected override void OnHide()
        {
            Debug.Log("Hide");
        }
        
        public override void OnDestroy()
        {
            this.Button.onClick.RemoveListener(this.RefreshTime);
        }
    }
}