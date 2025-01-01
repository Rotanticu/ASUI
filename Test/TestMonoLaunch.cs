using ASUI;
using LitDamper;
using R3;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TestMonoLaunch : MonoBehaviour
{
    [SerializeField]
    TestMainWindow mainWindow;
    public GameObject windowGameObject;
    public Button ShowHideButton;
    public Button SwitchStateButton;

    testASUIAnimatableValue testASUIAnimatableValue = new testASUIAnimatableValue();
    public void Awake()
    {
        LDamper.CreateDamper(
            () =>
            DamperSlider.normalizedValue,
            (value) =>
            DamperSlider.normalizedValue = (float)value,
            () =>
            ControllerSlider.normalizedValue).WithSpring(SpringType.SimpleSpring).WithHalfTime(0.1665d).RunWithoutBinding();
        //Observable.TimerFrame(0,1, UnityFrameProvider.FixedUpdate).Take(10).Subscribe((x)=>UpdateDamper());
        mainWindow.Init(windowGameObject);
        Observable<Unit> btnObservable = ShowHideButton.OnClickAsObservable();
        btnObservable.Subscribe((_) =>
        {
            if (mainWindow.WindowState is ASUIWindowState.IsShow or ASUIWindowState.IsShowAnimating)
            {
                mainWindow.Hide();
            }
            else
            {
                mainWindow.Show();
            }
        });
        SwitchStateButton.OnClickAsObservable().Subscribe((_) =>
        {
            //if (mainWindow.CurrentState == "state1")
            //    (mainWindow as IASUIStateSwitch).SwitchToState("state2");
            //else
            //    (mainWindow as IASUIStateSwitch).SwitchToState("state1");

            if (testASUIAnimatableValue.AnimationSpeed <= 0)
                testASUIAnimatableValue.AnimationSpeed = 0.01f;
            else
                testASUIAnimatableValue.AnimationSpeed = -0.01f;
        });

        testASUIAnimatableValue.StartValue = 1;
        testASUIAnimatableValue.EndValue = 0;
        testASUIAnimatableValue.CurrentValue = mainWindow.canvasGroup.alpha;
        testASUIAnimatableValue.Ease = Ease.InQuad;
        testASUIAnimatableValue.InterpolationTime = 0;
        Observable.TimerFrame(0, 1, UnityFrameProvider.Update).Subscribe((x) =>
        {
            if (testASUIAnimatableValue.IsAnimating)
            {
                var t = testASUIAnimatableValue.InterpolationTime;
                t += testASUIAnimatableValue.AnimationSpeed;
                if (t < 0)
                {
                    t = 0;
                    testASUIAnimatableValue.CurrentValue = testASUIAnimatableValue.StartValue;
                    return;
                }
                else if (t == 0)
                {
                    return;
                }
                else if (t > 1)
                {
                    t = 1;
                    testASUIAnimatableValue.CurrentValue = testASUIAnimatableValue.EndValue;
                    return;
                }
                else if (t == 1)
                {
                    return;
                }
                testASUIAnimatableValue.CurrentValue = Mathf.Lerp(testASUIAnimatableValue.StartValue, testASUIAnimatableValue.EndValue, EaseUtility.Evaluate(t, testASUIAnimatableValue.Ease));
                mainWindow.canvasGroup.alpha = testASUIAnimatableValue.CurrentValue;
                testASUIAnimatableValue.InterpolationTime = t;
            }
        });
    }

    public Slider ControllerSlider;
    public Slider DamperSlider;
    public Slider DamperVelocitySlider;


    //PID²ÎÊý
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

    public void Show() { mainWindow.Show(); }
    public void Hide() { mainWindow.Hide(); }
    public void Destroy() { mainWindow.Destroy(false); }

    public static bool Approximately(float a, float b,float precision)
    {
        return MathF.Abs(b - a) < MathF.Max(1E-06f * MathF.Max(MathF.Abs(a), MathF.Abs(b)), precision);
    }

}

