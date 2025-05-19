using LitDamper;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ASUI
{
    public interface IASUIAnimationBehavior
    {
        public Task PlayForward();
        public void Pause();
        public void Resume();
        public void ReSet();
        public void CompletedImmediately(bool withCompletedCallback);
        public void SetAnimationSpeed(float speed);
    }
}
