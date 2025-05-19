using R3;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ASUI
{
    /// <summary>
    /// ����״̬ö��
    /// </summary>
    public enum AnimationState
    {
        uninitialized, // δ��ʼ��
        Playing,     // ������
        Pause,      // ��ͣ
        Completed,   // �����
        Idle,        // ����
    }

    public enum AnimationUpdateType
    {
        /// <summary>
        /// //�������� Update ����ͬ�����£����ٶ��뵱ǰʱ��̶�һ�¡����ʱ��̶ȱ���������Ҳ����֮������ƥ��
        /// </summary>
        TimeScaleUpdate,
        /// <summary>
        /// /// //�������� Update ����ͬ�����£������ٶ��뵱ǰʱ��̶��޹ء����ʱ��̶ȱ�����������Ȼ������ͬ���ٶȲ���
        /// </summary>
        UnscaledTimeUpdate,
        /// <summary>
        /// /// //�������� FixedUpdate ����ͬ�����£���������ϵͳ���ֲ���һ�£�
        /// </summary>
        FixedUpdate,
    }

    public abstract class ASUIAnimationProcess<T> : IASUIAnimationBehavior
    {
        /// <summary>
        /// �����ĵ�ǰ״̬
        /// </summary>
        public AnimationState CurrentState { get; private set; } = AnimationState.uninitialized;

        /// <summary>
        /// һ���Զ������ǳ�����������Ҫ��β��ŵĶ���
        /// һ���Զ������ڲ�����ɺ��Զ�����
        /// ������������Ҫ��β��ŵĶ�����Ҫ�ֶ�����
        /// </summary>
        public bool IsOnce = false;

        public AnimationUpdateType AnimationUpdateType = AnimationUpdateType.TimeScaleUpdate;

        /// <summary>
        /// �������ٶ�
        /// </summary>
        private float animationSpeed = 1f;

        protected float originalSpeed;

        private T m_animationDriver;
        public T AnimationDriver
        {
            get => m_animationDriver;
            protected set => m_animationDriver = value;
        }
        /// <summary>
        /// ��ʼ���Ŷ���
        /// </summary>
        public async Task PlayForward()
        {
            if (m_animationDriver == null)
            {
                InitializeAnimationDriver(IsOnce, AnimationUpdateType, animationSpeed, AnimationDriverCompletedCallback);
            }
            if (m_animationDriver == null)
                return;
            CurrentState = AnimationState.Idle;
            StartAnimationDriver();
            CurrentState = AnimationState.Playing;

            await Observable.EveryUpdate()
                    .FirstAsync(_ => CurrentState == AnimationState.Completed || CurrentState == AnimationState.Idle);
        }

        /// <summary>
        /// ��ͣ����
        /// </summary>
        public virtual void Pause()
        {
            if (CurrentState == AnimationState.uninitialized)
                return;
            if (CurrentState == AnimationState.Pause)
                return;
            if (CurrentState == AnimationState.Playing)
            {
                SetAnimationDriverSpeed(0);
                CurrentState = AnimationState.Pause;
            }
        }

        /// <summary>
        /// ȡ����ͣ����
        /// </summary>
        public virtual void Resume()
        {
            if (CurrentState == AnimationState.uninitialized)
                return;
            if (CurrentState != AnimationState.Pause)
                return;
            SetAnimationDriverSpeed(animationSpeed);
            CurrentState = AnimationState.Playing;
        }

        /// <summary>
        /// ���̻ص���������ʼ״̬
        /// </summary>
        public virtual void ReSet()
        {
            if (CurrentState == AnimationState.uninitialized)
                return;
            ReSetAnimationDriver();
            CurrentState = AnimationState.Idle;
        }
        /// <summary>
        /// ������ɶ����������������Ľ���״̬
        /// withCompletedCallback: �Ƿ����CompletedCallback
        /// </summary>
        public virtual void CompletedImmediately(bool withCompletedCallback)
        {
            if (CurrentState == AnimationState.uninitialized)
                return;
            CompletedImmediatelyAnimationDriver(withCompletedCallback);
            CurrentState = AnimationState.Completed;
        }

        public virtual void SetAnimationSpeed(float speed)
        {
            if (CurrentState == AnimationState.uninitialized)
                return;
            animationSpeed = speed;
            if (CurrentState != AnimationState.Pause)
                SetAnimationDriverSpeed(animationSpeed);

        }

        ///// <summary>
        /// ���Ŷ��������ܱ���ϵĶ������Ե���
        ///damp�������ܵ��ţ����޸ĳ�ʼ����ֵ��Tween���Ե��ţ�Unity��Anim���Ե��ţ������߲��ܱ����
        ///// </summary>
        //public virtual async Task PlayBackwards()
        //{
        //    if (CurrentState == AnimationState.uninitialized)
        //        return;
        //    CompletedImmediately(false);
        //    SetAnimationSpeed(-AnimationSpeed);
        //    await PlayForward();
        //}

        private void AnimationDriverCompletedCallback()
        {
            CurrentState = AnimationState.Completed;
        }

        /// <summary>
        /// ��ʼ������������
        /// </summary>
        protected abstract void InitializeAnimationDriver(bool isOnce, AnimationUpdateType animationUpdateType, float animationSpeed, Action completedCallback);

        /// <summary>
        /// ��ʼ���Ŷ���
        /// </summary>
        protected abstract void StartAnimationDriver();

        /// <summary>
        /// ȡ�����������̻ص���������ʼ״̬
        /// </summary>
        protected abstract void ReSetAnimationDriver();

        /// <summary>
        /// ������ɶ����������������Ľ���״̬
        /// </summary>
        protected abstract void CompletedImmediatelyAnimationDriver(bool withCompletedCallback);

        /// <summary>
        /// ���ö��������ٶ�
        /// </summary>
        /// <param name="speed"></param>
        protected abstract void SetAnimationDriverSpeed(float speed);

        /// <summary>
        /// ���ٶ���������
        /// </summary>
        public abstract void Kill();
    }

}
