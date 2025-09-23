using UnityEngine;
using UnityEngine.UI;

namespace ASUI.Test
{
    /// <summary>
    /// 自动创建Burst性能测试UI的脚本
    /// 在编辑器中运行此脚本来创建测试界面
    /// </summary>
    public class BurstPerformanceUICreator : MonoBehaviour
    {
        [ContextMenu("创建Burst性能测试UI")]
        public void CreateBurstPerformanceUI()
        {
            // 创建Canvas
            GameObject canvasGO = new GameObject("BurstPerformanceTestCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // 创建背景面板
            GameObject backgroundGO = new GameObject("Background");
            backgroundGO.transform.SetParent(canvasGO.transform, false);
            
            RectTransform backgroundRect = backgroundGO.AddComponent<RectTransform>();
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;
            
            Image backgroundImage = backgroundGO.AddComponent<Image>();
            backgroundImage.color = new Color(0, 0, 0, 0.8f);
            
            // 创建主面板
            GameObject mainPanelGO = new GameObject("MainPanel");
            mainPanelGO.transform.SetParent(backgroundGO.transform, false);
            
            RectTransform mainPanelRect = mainPanelGO.AddComponent<RectTransform>();
            mainPanelRect.anchorMin = new Vector2(0.1f, 0.1f);
            mainPanelRect.anchorMax = new Vector2(0.9f, 0.9f);
            mainPanelRect.offsetMin = Vector2.zero;
            mainPanelRect.offsetMax = Vector2.zero;
            
            Image mainPanelImage = mainPanelGO.AddComponent<Image>();
            mainPanelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            
            // 添加圆角效果
            mainPanelGO.AddComponent<Mask>();
            
            // 创建标题
            GameObject titleGO = new GameObject("Title");
            titleGO.transform.SetParent(mainPanelGO.transform, false);
            
            RectTransform titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.85f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = new Vector2(20, 0);
            titleRect.offsetMax = new Vector2(-20, -10);
            
            Text titleText = titleGO.AddComponent<Text>();
            titleText.text = "Burst性能测试工具";
            titleText.fontSize = 32;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            
            // 创建控制面板
            GameObject controlPanelGO = new GameObject("ControlPanel");
            controlPanelGO.transform.SetParent(mainPanelGO.transform, false);
            
            RectTransform controlPanelRect = controlPanelGO.AddComponent<RectTransform>();
            controlPanelRect.anchorMin = new Vector2(0, 0.7f);
            controlPanelRect.anchorMax = new Vector2(1, 0.85f);
            controlPanelRect.offsetMin = new Vector2(20, 0);
            controlPanelRect.offsetMax = new Vector2(-20, -10);
            
            // 创建迭代次数滑块
            GameObject sliderGO = new GameObject("IterationSlider");
            sliderGO.transform.SetParent(controlPanelGO.transform, false);
            
            RectTransform sliderRect = sliderGO.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0, 0.5f);
            sliderRect.anchorMax = new Vector2(0.7f, 1);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = new Vector2(-10, 0);
            
            Slider slider = sliderGO.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.5f;
            
            // 滑块背景
            GameObject sliderBackgroundGO = new GameObject("Background");
            sliderBackgroundGO.transform.SetParent(sliderGO.transform, false);
            
            RectTransform sliderBackgroundRect = sliderBackgroundGO.AddComponent<RectTransform>();
            sliderBackgroundRect.anchorMin = Vector2.zero;
            sliderBackgroundRect.anchorMax = Vector2.one;
            sliderBackgroundRect.offsetMin = Vector2.zero;
            sliderBackgroundRect.offsetMax = Vector2.zero;
            
            Image sliderBackgroundImage = sliderBackgroundGO.AddComponent<Image>();
            sliderBackgroundImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            // 滑块填充
            GameObject sliderFillGO = new GameObject("Fill Area");
            sliderFillGO.transform.SetParent(sliderGO.transform, false);
            
            RectTransform sliderFillAreaRect = sliderFillGO.AddComponent<RectTransform>();
            sliderFillAreaRect.anchorMin = Vector2.zero;
            sliderFillAreaRect.anchorMax = Vector2.one;
            sliderFillAreaRect.offsetMin = Vector2.zero;
            sliderFillAreaRect.offsetMax = Vector2.zero;
            
            GameObject sliderFillGO2 = new GameObject("Fill");
            sliderFillGO2.transform.SetParent(sliderFillGO.transform, false);
            
            RectTransform sliderFillRect = sliderFillGO2.AddComponent<RectTransform>();
            sliderFillRect.anchorMin = Vector2.zero;
            sliderFillRect.anchorMax = Vector2.one;
            sliderFillRect.offsetMin = Vector2.zero;
            sliderFillRect.offsetMax = Vector2.zero;
            
            Image sliderFillImage = sliderFillGO2.AddComponent<Image>();
            sliderFillImage.color = new Color(0.2f, 0.6f, 1f, 1f);
            
            // 滑块手柄
            GameObject sliderHandleGO = new GameObject("Handle Slide Area");
            sliderHandleGO.transform.SetParent(sliderGO.transform, false);
            
            RectTransform sliderHandleAreaRect = sliderHandleGO.AddComponent<RectTransform>();
            sliderHandleAreaRect.anchorMin = Vector2.zero;
            sliderHandleAreaRect.anchorMax = Vector2.one;
            sliderHandleAreaRect.offsetMin = Vector2.zero;
            sliderHandleAreaRect.offsetMax = Vector2.zero;
            
            GameObject sliderHandleGO2 = new GameObject("Handle");
            sliderHandleGO2.transform.SetParent(sliderHandleGO.transform, false);
            
            RectTransform sliderHandleRect = sliderHandleGO2.AddComponent<RectTransform>();
            sliderHandleRect.sizeDelta = new Vector2(20, 20);
            
            Image sliderHandleImage = sliderHandleGO2.AddComponent<Image>();
            sliderHandleImage.color = Color.white;
            
            // 设置滑块的引用
            slider.fillRect = sliderFillRect;
            slider.handleRect = sliderHandleRect;
            
            // 创建迭代次数标签
            GameObject iterationLabelGO = new GameObject("IterationLabel");
            iterationLabelGO.transform.SetParent(controlPanelGO.transform, false);
            
            RectTransform iterationLabelRect = iterationLabelGO.AddComponent<RectTransform>();
            iterationLabelRect.anchorMin = new Vector2(0.7f, 0.5f);
            iterationLabelRect.anchorMax = new Vector2(1, 1);
            iterationLabelRect.offsetMin = new Vector2(10, 0);
            iterationLabelRect.offsetMax = Vector2.zero;
            
            Text iterationLabelText = iterationLabelGO.AddComponent<Text>();
            iterationLabelText.text = "迭代次数: 500,000";
            iterationLabelText.fontSize = 16;
            iterationLabelText.color = Color.white;
            iterationLabelText.alignment = TextAnchor.MiddleLeft;
            
            // 创建按钮面板
            GameObject buttonPanelGO = new GameObject("ButtonPanel");
            buttonPanelGO.transform.SetParent(controlPanelGO.transform, false);
            
            RectTransform buttonPanelRect = buttonPanelGO.AddComponent<RectTransform>();
            buttonPanelRect.anchorMin = new Vector2(0, 0);
            buttonPanelRect.anchorMax = new Vector2(1, 0.5f);
            buttonPanelRect.offsetMin = new Vector2(0, 0);
            buttonPanelRect.offsetMax = new Vector2(0, -5);
            
            // 创建运行测试按钮
            GameObject runButtonGO = new GameObject("RunTestButton");
            runButtonGO.transform.SetParent(buttonPanelGO.transform, false);
            
            RectTransform runButtonRect = runButtonGO.AddComponent<RectTransform>();
            runButtonRect.anchorMin = new Vector2(0, 0);
            runButtonRect.anchorMax = new Vector2(0.5f, 1);
            runButtonRect.offsetMin = Vector2.zero;
            runButtonRect.offsetMax = new Vector2(-5, 0);
            
            Image runButtonImage = runButtonGO.AddComponent<Image>();
            runButtonImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
            
            Button runButton = runButtonGO.AddComponent<Button>();
            
            // 按钮文字
            GameObject runButtonTextGO = new GameObject("Text");
            runButtonTextGO.transform.SetParent(runButtonGO.transform, false);
            
            RectTransform runButtonTextRect = runButtonTextGO.AddComponent<RectTransform>();
            runButtonTextRect.anchorMin = Vector2.zero;
            runButtonTextRect.anchorMax = Vector2.one;
            runButtonTextRect.offsetMin = Vector2.zero;
            runButtonTextRect.offsetMax = Vector2.zero;
            
            Text runButtonText = runButtonTextGO.AddComponent<Text>();
            runButtonText.text = "运行测试";
            runButtonText.fontSize = 18;
            runButtonText.color = Color.white;
            runButtonText.alignment = TextAnchor.MiddleCenter;
            runButtonText.fontStyle = FontStyle.Bold;
            
            // 创建清除按钮
            GameObject clearButtonGO = new GameObject("ClearButton");
            clearButtonGO.transform.SetParent(buttonPanelGO.transform, false);
            
            RectTransform clearButtonRect = clearButtonGO.AddComponent<RectTransform>();
            clearButtonRect.anchorMin = new Vector2(0.5f, 0);
            clearButtonRect.anchorMax = new Vector2(1, 1);
            clearButtonRect.offsetMin = new Vector2(5, 0);
            clearButtonRect.offsetMax = Vector2.zero;
            
            Image clearButtonImage = clearButtonGO.AddComponent<Image>();
            clearButtonImage.color = new Color(0.8f, 0.2f, 0.2f, 1f);
            
            Button clearButton = clearButtonGO.AddComponent<Button>();
            
            // 按钮文字
            GameObject clearButtonTextGO = new GameObject("Text");
            clearButtonTextGO.transform.SetParent(clearButtonGO.transform, false);
            
            RectTransform clearButtonTextRect = clearButtonTextGO.AddComponent<RectTransform>();
            clearButtonTextRect.anchorMin = Vector2.zero;
            clearButtonTextRect.anchorMax = Vector2.one;
            clearButtonTextRect.offsetMin = Vector2.zero;
            clearButtonTextRect.offsetMax = Vector2.zero;
            
            Text clearButtonText = clearButtonTextGO.AddComponent<Text>();
            clearButtonText.text = "清除结果";
            clearButtonText.fontSize = 18;
            clearButtonText.color = Color.white;
            clearButtonText.alignment = TextAnchor.MiddleCenter;
            clearButtonText.fontStyle = FontStyle.Bold;
            
            // 创建结果显示区域
            GameObject resultPanelGO = new GameObject("ResultPanel");
            resultPanelGO.transform.SetParent(mainPanelGO.transform, false);
            
            RectTransform resultPanelRect = resultPanelGO.AddComponent<RectTransform>();
            resultPanelRect.anchorMin = new Vector2(0, 0);
            resultPanelRect.anchorMax = new Vector2(1, 0.7f);
            resultPanelRect.offsetMin = new Vector2(20, 20);
            resultPanelRect.offsetMax = new Vector2(-20, -10);
            
            // 添加滚动视图
            ScrollRect scrollRect = resultPanelGO.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            
            // 创建内容区域
            GameObject contentGO = new GameObject("Content");
            contentGO.transform.SetParent(resultPanelGO.transform, false);
            
            RectTransform contentRect = contentGO.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;
            contentRect.pivot = new Vector2(0.5f, 1);
            
            // 创建结果文本
            GameObject resultTextGO = new GameObject("ResultText");
            resultTextGO.transform.SetParent(contentGO.transform, false);
            
            RectTransform resultTextRect = resultTextGO.AddComponent<RectTransform>();
            resultTextRect.anchorMin = Vector2.zero;
            resultTextRect.anchorMax = Vector2.one;
            resultTextRect.offsetMin = Vector2.zero;
            resultTextRect.offsetMax = Vector2.zero;
            
            Text resultText = resultTextGO.AddComponent<Text>();
            resultText.text = "点击'运行测试'开始性能测试...";
            resultText.fontSize = 14;
            resultText.color = Color.white;
            resultText.alignment = TextAnchor.UpperLeft;
            resultText.horizontalOverflow = HorizontalWrapMode.Wrap;
            resultText.verticalOverflow = VerticalWrapMode.Overflow;
            
            // 设置滚动视图的引用
            scrollRect.content = contentRect;
            scrollRect.viewport = resultPanelRect;
            
            // 添加BurstPerformanceUITest组件
            BurstPerformanceUITest testScript = mainPanelGO.AddComponent<BurstPerformanceUITest>();
            
            // 设置引用
            testScript.resultText = resultText;
            testScript.runTestButton = runButton;
            testScript.clearButton = clearButton;
            testScript.iterationSlider = slider;
            testScript.iterationLabel = iterationLabelText;
            
            Debug.Log("Burst性能测试UI创建完成！");
        }
    }
}
