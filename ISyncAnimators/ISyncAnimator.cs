using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CodeDucky.Syncing.AnimatorController
{
    [RequireComponent(typeof(Animator))]
    public class ISyncAnimator : MonoBehaviour
    {
        public bool developerMode = false;
        public float animationSpeedModifier;

        private Animator _animator;
        private AnimationClip[] _clips;

        void Start()
        {
            if(developerMode){
                Debug.LogWarning($"ISyncAnimator : Warning : {transform.name} is set to Developer Mode. Disable this for Production Deploy.");
            }

            // Get Animator & Clips
            _animator = GetComponent<Animator>();
            _clips = _animator.runtimeAnimatorController.animationClips;

        }

        void Update()
        {
            if(developerMode)
            {
                int clipIndex = 0;

                float _currLength = _clips[clipIndex].length;
                float _loopTime = Time.timeSinceLevelLoad;
                float _animMod = (_loopTime * animationSpeedModifier) % _currLength;

                SetAnimationProgress(_animMod);
            }
        }

        public void syncAnimationByValue(float time, int clipIndex)
        {
            float _currLength = _clips[clipIndex].length;
            float _animMod = (time * animationSpeedModifier) % _currLength;
            SetAnimationProgress(_animMod);
        }
    
        public void SetAnimationProgress(float ratio)
        {
            // update animator's progress
            _animator.SetFloat("progress", ratio);
        }
    }
}