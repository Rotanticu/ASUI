using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;

namespace ASUI
{
    [Serializable]
    public class TestMainWindow : WidgetsBase
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
            this.HeadText.text = $"���ڵ�ʱ����{DateTime.Now.ToString("HH:mm:ss")}";
        }

        public override void OnStartTransition(string transitionName)
        {
            base.OnStartTransition(transitionName);
            if(transitionName == "Show")
            {
                this.OnShow();
            }
            if(transitionName == "Hide")
            {
                this.OnHide();
            }
        }
        public void OnShow()
        {
            this.HeadText.text = $"���ڵ�ʱ����{DateTime.Now.ToString("HH:mm:ss")}";
        }
        public void OnHide()
        {
            Debug.Log("Hide");
        }
        public override void OnDestroy()
        {
            this.Button.onClick.RemoveListener(this.RefreshTime);
        }
    }
}

