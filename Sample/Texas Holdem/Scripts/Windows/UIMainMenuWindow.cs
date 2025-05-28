using ASUI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using R3;
using R3.Triggers;

public class UIMainMenuWindow : ASUIWindowBase
{
    public override void OnInit()
    {

    }
    public override void OnShow()
    {
        PlayableDirector playableDirector = this.GameObject.GetComponent<PlayableDirector>();
        PlayableGraph graph = PlayableGraph.Create("MainMenuScale");
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 60;
        clip.wrapMode = WrapMode.Once;
        clip.name = "MainMenuScale";
        clip.SetCurve("tex_title", typeof(Transform), "localScale.x", new AnimationCurve(new Keyframe(0f, 0.2f), new Keyframe(10f, 1f)));
        clip.SetCurve("tex_title", typeof(Transform), "localScale.y", new AnimationCurve(new Keyframe(0f, 0.2f), new Keyframe(5f, 1f)));
        clip.SetCurve("tex_title", typeof(Transform), "localScale.z", new AnimationCurve(new Keyframe(0f, 0.2f), new Keyframe(3f, 1f)));
        AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(graph, clip);
        var output = AnimationPlayableOutput.Create(graph, "AnimationOutput", this.GameObject.AddComponent<Animator>());
        output.SetSourcePlayable(clipPlayable);
        graph.Play();
        this.GameObject.transform.OnDestroyAsObservable().Subscribe(_ => graph.Destroy());
        // int count = playableDirector.playableGraph.GetPlayableCount();
        // Debug.Log(count);
        // for (int i = 0; i < count; i++)
        // {
        //     Debug.Log(playableDirector.playableGraph.GetRootPlayable(i).GetHandle().ToString());
        //     //playableDirector.playableGraph.GetRootPlayable(i).SetSpeed(0.5);
        // }
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
