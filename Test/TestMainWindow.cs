using LitDamper;
using LitDamper.Extensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ASUI
{
    [Serializable]
    public class TestMainWindow : ASUIWindowBase
    {
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI HeadText;
        
        public float t;
        public Button Button { get; private set; }
        private bool m_IsUpdateFade = false;
        [ContextMenu("OnInit")]
        public override void OnInit()
        {
            this.styleState = this.Transform.GetComponent<ASUIStyleState>();
            this.canvasGroup = this.styleState.GetComponentByUIName<CanvasGroup>("Canvas");
            this.HeadText = this.styleState.GetComponentByUIName<TextMeshProUGUI>("HeadText");
            this.Button = this.styleState.GetComponentByUIName<Button>("Button");
            this.Button.onClick.AddListener(this.RefreshTime);
        }

        public void RefreshTime()
        {
            this.HeadText.text = $"���ڵ�ʱ����{DateTime.Now.ToString("HH:mm:ss")}";
        }
        public override void OnShow()
        {
            this.HeadText.text = $"���ڵ�ʱ����{DateTime.Now.ToString("HH:mm:ss")}";
        }
        private float fadeDuration = 2f;
        private LitDamper.DamperHandle showMotion;
        public override void PlayShowAnimation()
        {
            m_IsUpdateFade = true;
            float currentAlpha = this.canvasGroup.alpha;
            float duration = (1.0f - currentAlpha) * fadeDuration;
            if (showMotion.IsActive())
            {
                showMotion.Cancel();
                //hideMotion.Complete();
            }
            if (hideMotion.IsActive())
            {
                hideMotion.Cancel();
                //hideMotion.Complete();
            }
        }
        public override void ShowAnimationCompleted()
        {
            base.ShowAnimationCompleted();
            m_IsUpdateFade = false;
        }
        public override void OnHide()
        {
            Debug.Log("Hide");
        }
        private LitDamper.DamperHandle hideMotion;
        public override void PlayHideAnimation()
        {
            m_IsUpdateFade = true;
            float currentAlpha = this.canvasGroup.alpha;
            float duration = (currentAlpha) * fadeDuration;
            if (showMotion.IsActive())
            {
                showMotion.Cancel();
                //hideMotion.Complete();
            }
            if (hideMotion.IsActive())
            {
                hideMotion.Cancel();
                //hideMotion.Complete();
            }
        }
        public override void HideAnimationCompleted()
        {
            base.HideAnimationCompleted();
            m_IsUpdateFade = false;
        }
        public void Update()
        {
            if (!m_IsUpdateFade)
            {
                return;
            }
            if (this.WindowState == ASUIWindowState.IsShowAnimating)
            {
                if (this.canvasGroup.alpha < 1)
                {
                    this.canvasGroup.alpha += 0.01f;
                }
                else
                {
                    this.ShowAnimationCompleted();
                }
            }
            if (this.WindowState == ASUIWindowState.IsHideAnimating)
            {
                if (this.canvasGroup.alpha > 0)
                {
                    this.canvasGroup.alpha -= 0.01f;
                }
                else
                {
                    this.HideAnimationCompleted();
                }
            }
        }
        public override void OnDestroy()
        {
            this.Button.onClick.RemoveListener(this.RefreshTime);
        }
        public override void ApplyStyle()
        {

        }
    }
}
