using ASUI;
using LitDamper;
using R3;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Threading.Tasks;

public class TestMonoLaunch : MonoBehaviour
{
    [SerializeField]
    TestMainWindow mainWindow;
    public GameObject windowGameObject;
    public Button ShowHideButton;
    public Button SwitchStateButton;

    public void Awake()
    {
        //DoTween����
        var tweener = DOTween.To(()=> DamperVelocitySlider.normalizedValue, (x) => DamperVelocitySlider.normalizedValue = x, DamperSlider.normalizedValue, 5);
        tweener.SetAutoKill(false);
        DamperSlider.OnValueChangedAsObservable().Subscribe((value) =>
        {
            tweener.ChangeStartValue(DamperVelocitySlider.normalizedValue);
            tweener.ChangeEndValue(DamperSlider.normalizedValue, true);
            if (ControllerSlider.normalizedValue < 0.5f)
            {
                if (!tweener.IsPlaying())
                    tweener.PlayBackwards();
            }
            else
                if (!tweener.IsPlaying())
                tweener.PlayForward();
        });

        //LDamper����
        LDamper.CreateDamper(
            () =>
            DamperSlider.normalizedValue,
            (value) =>
            DamperSlider.normalizedValue = (float)value,
            () =>
            ControllerSlider.normalizedValue).WithSpring(SpringType.SimpleSpring).WithHalfTime(0.1665d).RunWithoutBinding();
        //Observable.TimerFrame(0,1, UnityFrameProvider.FixedUpdate).Take(10).Subscribe((x)=>UpdateDamper());

        //Observable����
        mainWindow.Init(windowGameObject);
        Observable<Unit> btnObservable = ShowHideButton.OnClickAsObservable();
        btnObservable.Subscribe(async (_) =>
        {
            if (mainWindow.WidgetState is WidgetState.Show or WidgetState.Entering)
            {
                await mainWindow.Hide();
            }
            else
            {
                await mainWindow.Show();
            }
        });
    }

    public Slider ControllerSlider;
    public Slider DamperSlider;
    public Slider DamperVelocitySlider;


    //PID����
    private bool isInMotion = false;
    private float motionTime = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dampVelocity = 0;
        xi = DamperSlider.normalizedValue;
    }
    // Update is called once per frame
    void Update()
    {
        //UpdateDamper();
    }

    public double HalfLife = 0.1665d;
    public double TargetVelocity = 1d;
    public double TargetTime = 1d;
    public double Apprehension = 2d;
    private double dampVelocity;
    private double xi;
    //private double vi;
    public void UpdateDamper()
    {
        double startValue = DamperSlider.normalizedValue;
        //DamperSlider.normalizedValue = (float)DamperUtility.DoubleSpringDamperImplicit(startValue,ref dampVelocity,ref xi, ref vi,ControllerSlider.normalizedValue, HalfLife,(double)Time.deltaTime);
        //DamperSlider.normalizedValue = (float)DamperUtility.VelocitySpringDamperImplicit(startValue, ref dampVelocity, ref xi, ControllerSlider.normalizedValue, TargetVelocity, HalfLife, (double)Time.deltaTime);
        DamperSlider.normalizedValue = (float)DamperUtility.SimpleSpringDamperImplicit(startValue, ref dampVelocity, ControllerSlider.normalizedValue, HalfLife, (double)Time.deltaTime);
        if (DamperVelocitySlider.maxValue < dampVelocity)
        {
            DamperVelocitySlider.maxValue = (float)dampVelocity * 2f;
        }
        DamperVelocitySlider.value = Math.Abs((float)dampVelocity);

        if(Approximately(DamperSlider.normalizedValue,ControllerSlider.normalizedValue,1e-3f) && isInMotion)
        {
            isInMotion = false;
            Debug.Log($"motionTime = {motionTime}");
            motionTime = 0;
        }
        if (!Approximately(DamperSlider.normalizedValue, ControllerSlider.normalizedValue, 1e-3f) && !isInMotion)
        {
            isInMotion = true;
        }

        if(isInMotion)
        {
            motionTime += Time.deltaTime;
        }
    }

    public void OnDestroy()
    {
    }

    public async Task Show() { await mainWindow.Show(); }
    public async Task Hide() { await mainWindow.Hide(); }
    public async Task Destroy() { await mainWindow.Destroy(false); }

    public static bool Approximately(float a, float b,float precision)
    {
        return MathF.Abs(b - a) < MathF.Max(1E-06f * MathF.Max(MathF.Abs(a), MathF.Abs(b)), precision);
    }

}

