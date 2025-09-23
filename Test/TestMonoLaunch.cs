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
    public Slider DamperVelocitySlider;

    public RectTransform followTarget;
    
    // Spring动画变量
    private MotionHandle followMotion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var motion = LitMotion.LMotion.Spring.Create(DamperSlider.normalizedValue, ControllerSlider.normalizedValue, 1, LitMotion.SpringOptions.Underdamped).WithLoops(-1).Bind(x => DamperSlider.normalizedValue = x);
        ControllerSlider.onValueChanged.AddListener(x => motion.SetEndValue<float, LitMotion.SpringOptions>(x));
    }
    
    // Update is called once per frame
    void Update()
    {
        // 检测鼠标右键按下
        if (Input.GetMouseButton(1)) // 右键按下
        {
            // 获取鼠标在屏幕上的位置
            Vector2 mouseScreenPos = Input.mousePosition;
            
            if (followMotion == MotionHandle.None)
            {
                // 创建2D Spring动画，让followTarget跟随鼠标位置
                followMotion = LitMotion.LMotion.Spring.Create(
                    (Vector2)followTarget.position,  // 起始位置
                    mouseScreenPos,                  // 目标位置（鼠标位置）
                    1.0f,                           // 持续时间
                    LitMotion.SpringOptions.Underdamped  // 使用欠阻尼，有弹性效果
                ).WithLoops(-1)
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

    public static bool Approximately(float a, float b,float precision)
    {
        return MathF.Abs(b - a) < MathF.Max(1E-06f * MathF.Max(MathF.Abs(a), MathF.Abs(b)), precision);
    }

}

