using ASUI;
//using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using R3;
using R3.Triggers;
using LitMotion;
using LitMotion.Extensions;

public class UIMainMenuWindow : ASUIWindowBase
{
    public UIMainTitle mainTitle;
    public override void OnInit()
    {
        mainTitle = new UIMainTitle();
        mainTitle.Init(this.StyleState.GetComponentByUIName<RectTransform>("tex_title").gameObject);
    }
    public override void OnShow()
    {
        //mainTitle.TexTitle.AnimateColorTo(Color.red, 1f);
        LMotion.Create(0f,1f,3f).ToObservable().Subscribe(x=>{
            Debug.Log(x);
        });
        // 使用DOTween实现动画
        //这部分相当于AnimBuilder
        // DOTweenAnimation dotweenAnimation = this.GameObject.AddComponent<DOTweenAnimation>();
        // dotweenAnimation.animationType = DOTweenAnimation.AnimationType.Scale;
        // dotweenAnimation.targetType = DOTweenAnimation.TargetType.RectTransform;
        // dotweenAnimation.targetIsSelf = false;
        // dotweenAnimation.target = this.Transform.Find("tex_title").GetComponent<RectTransform>();
        // dotweenAnimation.targetGO = dotweenAnimation.target.gameObject;
        // dotweenAnimation.isValid = true;
        // dotweenAnimation.duration = 1f;
        // dotweenAnimation.isFrom = true;
        // dotweenAnimation.endValueV3 = new Vector3(0.2f, 0.2f, 1f);
        // //RecreateTween会让当前状态直接成为动画的初始状态，这点可能需要修改
        // dotweenAnimation.RecreateTween();
        // //dotweenAnimation.tween相当于AnimProgress
        // dotweenAnimation.tween.Play();


        // 使用LitMotion实现动画
        //相当于AnimBuilder
        //var builder = LMotion.Create(new Vector3(0.2f, 0.2f, 1f), new Vector3(1f, 1f, 1f), 1f);
        //相当于AnimProgress
        //var motionHandle = builder.BindToLocalScale(this.Transform.Find("tex_title").GetComponent<RectTransform>());

        // 使用PlayableDirector实现动画
        //playableDirector相当于AnimBuilder
        // PlayableDirector playableDirector = this.GameObject.GetComponent<PlayableDirector>();
        // PlayableGraph graph = PlayableGraph.Create("MainMenuScale");
        // AnimationClip clip = new AnimationClip();
        // clip.frameRate = 60;
        // clip.wrapMode = WrapMode.Once;
        // clip.name = "MainMenuScale";
        // clip.SetCurve("tex_title", typeof(Transform), "localScale.x", new AnimationCurve(new Keyframe(0f, 0.2f), new Keyframe(1f, 1f)));
        // clip.SetCurve("tex_title", typeof(Transform), "localScale.y", new AnimationCurve(new Keyframe(0f, 0.2f), new Keyframe(1f, 1f)));
        // clip.SetCurve("tex_title", typeof(Transform), "localScale.z", new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f)));
        // AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(graph, clip);
        // var output = AnimationPlayableOutput.Create(graph, "AnimationOutput", this.GameObject.AddComponent<Animator>());
        // output.SetSourcePlayable(clipPlayable);
        // //graph相当于AnimProgress
        // graph.Play();
        // this.GameObject.transform.OnDestroyAsObservable().Subscribe(_ => graph.Destroy());


    }
    public override void PlayShowAnimation()
    {
    }
    public override void ShowAnimationCompleted()
    {
        base.ShowAnimationCompleted();
    }
    public override void OnHide()
    {
    }
    public override void PlayHideAnimation()
    {
    }
    public override void HideAnimationCompleted()
    {
        base.HideAnimationCompleted();
    }
    public void Update()
    {
    }
    public override void OnDestroy()
    {
        //this.Button.onClick.RemoveListener(this.RefreshTime);
    }
    public override void ApplyStyle()
    {

    }
}
