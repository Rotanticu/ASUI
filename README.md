#ASUI产品文档
##Overview
- ASUI是一个为Unity引擎引擎开发，基于动画和样式的响应式异步UI库。
- ASUI受Flutter Widget启发，基于Widget构建UI元素。
- ASUI充分利用R3和LitMotion、DoTween的提供的功能，方便的使用响应编程创建异步UI动画，让您轻松处理UI动画的排序、播放和异步问题，摆脱回调地狱。

##Features
- 流畅的异步编程体验
- 使用R3实现响应式UI编程体验
- 深度集成LitMotion实现多种常用动画
- 支持Damper、LitMotion、DoTween、Unity Animation、自定义动画驱动器，多种动画驱动方式
- 支持UI智能预加载，智能缓存

##Requirements
- Unity 2021.3 or later
- R3
- LitMotion
- uGUI

##Getting Started
- 以下是一个最简单示例代码
  ```csharp
  public class HeadText : WidgetsBase
  { 
    public TextMeshPro TexTitle;

    public override void OnInit()
    {
        this.TexTitle = this.StyleState?.GetComponentByUIName<TextMeshPro>("Tex_Title");
    }

    public HeadText SetText(string text)
    {
        TexTitle.text = text;
        return this;
    }
  }

  public class HeadWindow : WindowBase
  { 
    private HeadText headTextBig;
    private HeadText headTextSmall;

    public override void OnShow()
    {
        headTextBig.SetText("Look This!").Show().DoFade(0,1).DoScale(0.3,2);
        headTextSmall.SetText("Is Amazing?").DoText().Delay(1.5).Then().DoText("Yes!I Know!");
    }
  }
  
  public class TestMonoLaunch : MonoBehaviour
  {
    void Start()
    {
        ASUI.WindowMagager.CreateWindow<HeadWindow>().Show();
    }
  }
  ```


















  ```