using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimationController: MonoBehaviour
{
    Animator anim;

    Rigidbody2D rb;

    struct Animations{
        public AnimationClip anim_left {private set; get;}
        public AnimationClip anim_right {private set; get;}
        public AnimationClip anim_forward {private set; get;}
        public AnimationClip anim_backward {private set; get;}
        public AnimationClip anim_idle {private set; get;}

        public AnimationClip current_clip;
        public AnimationClip previous_clip;

        public Animations(string spriteResourceName){
            anim_left = Resources.Load<AnimationClip>("Sprites/Animations/" + spriteResourceName + "_left");
            anim_right = Resources.Load<AnimationClip>("Sprites/Animations/" + spriteResourceName + "_right");
            anim_forward = Resources.Load<AnimationClip>("Sprites/Animations/" + spriteResourceName + "_forward");
            anim_backward = Resources.Load<AnimationClip>("Sprites/Animations/" + spriteResourceName + "_backward");
            anim_idle = Resources.Load<AnimationClip>("Sprites/Animations/" + spriteResourceName + "_idle");

            current_clip = anim_idle;
            previous_clip = anim_idle;
        }
    }

    Animations animations;
    
    public EntityAnimationController Constructor(Animator attachedAnim, string spriteName, Rigidbody2D entityRb){
        anim = attachedAnim;

        animations = new Animations(spriteName);

        rb = entityRb;

        return this;
    }

    private void FixedUpdate (){
        float x = 0, y = 0;

        if (rb != null){
            x = Mathf.Abs(rb.velocity.x);
            y = Mathf.Abs(rb.velocity.y);

            animations.previous_clip = animations.current_clip;

            if (rb.velocity.magnitude == 0){
                animations.current_clip = animations.anim_idle;
            }
            else if (x > y){
                animations.current_clip = animations.anim_left;

            }
            else if (x < y){
                animations.current_clip = animations.anim_forward;

            }

            SwitchAnims();
        }
    }

    private void SwitchAnims(){
        if (animations.previous_clip == animations.current_clip){
            return;
        }
        else{

            anim.StopPlayback();
            anim.Play(animations.current_clip.name);
        }
    }

}
