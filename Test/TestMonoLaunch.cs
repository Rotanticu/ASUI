using ASUI;
using LitMotion;
using R3;
using System;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;
using TMPro;
using System.Threading.Tasks;
using LitMotion.Extensions;
using UnityEngine.InputSystem;


public class TestMonoLaunch : MonoBehaviour
{
    [SerializeField]
    TestMainWindow mainWindow;
    public GameObject windowGameObject;
    public Button ShowHideButton;
    public Button SwitchStateButton;

    public void Awake()
    {

    }

    public Slider ControllerSlider;
    public Slider DamperSlider;
    public Slider DamperStiffnessSlider;

    public RectTransform followTarget;


    private MotionHandle springMotion;
    // Spring动画变量
    private MotionHandle followMotion;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var springOptions = LitMotion.SpringOptions.Underdamped;
        springMotion = LitMotion.LMotion.Spring.Create(DamperSlider.normalizedValue, ControllerSlider.normalizedValue, springOptions).WithLoops(-1,LoopType.Incremental).Bind(x => DamperSlider.normalizedValue = x);
        ControllerSlider.onValueChanged.AddListener(x => springMotion.SetEndValue<float, LitMotion.SpringOptions>(x));
        DamperStiffnessSlider.onValueChanged.AddListener(x => springMotion.GetOptions<float, LitMotion.SpringOptions>().DampingRatio = x);
        
        // 设置状态切换按钮事件
        if (SwitchStateButton != null)
        {
            SwitchStateButton.onClick.AddListener(SwitchToNextState);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 检测鼠标右键按下
        if (Mouse.current.rightButton.isPressed) // 右键按下
        {
            // 获取鼠标在屏幕上的位置
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

            if (followMotion == MotionHandle.None)
            {
                // 创建2D Spring动画，让followTarget跟随鼠标位置
                followMotion = LitMotion.LMotion.Spring.Create(
                    (Vector2)followTarget.position,  // 起始位置
                    mouseScreenPos,                  // 目标位置（鼠标位置）
                    LitMotion.SpringOptions.Underdamped                           // 使用欠阻尼，有弹性效果
                ).WithLoops(-1,LitMotion.LoopType.Incremental)
                .Bind(pos => followTarget.position = pos);
            }
            else
            {
                // 更新目标位置
                followMotion.SetEndValue<Vector2, LitMotion.SpringOptions>(mouseScreenPos);
            }
        }
    }
    public ReactiveProperty<Color> BackgroundColor = new ReactiveProperty<Color>(Color.white);

    public void SetBackgroundColor()
    {
        Func<Color> colorFunc = () =>
        {
            return UnityEngine.Random.ColorHSV();
        };
    }

    public async Task Show() { await mainWindow.Show(); }
    public async Task Hide() { await mainWindow.Hide(); }
    public async Task Destroy() { await mainWindow.Destroy(false); }

    /// <summary>
    /// 切换到下一个状态
    /// </summary>
    public void SwitchToNextState()
    {
        
    }

    public static bool Approximately(float a, float b, float precision)
    {
        return MathF.Abs(b - a) < MathF.Max(1E-06f * MathF.Max(MathF.Abs(a), MathF.Abs(b)), precision);
    }

}

