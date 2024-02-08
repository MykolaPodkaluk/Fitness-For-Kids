using System.Collections.Generic;
using UnityEngine;

public static class RuntimeAnimatorProvider
{
    static List<string> animationKeys = new List<string> { "Exercise 01", "Exercise 02", "Exercise 03", "Exercise 04", "Exercise 05", "Exercise 06" };
    public static List<string> AnimationKeys => animationKeys;

    public static void SetRuntimeAnimatorController(Animator animator, List<AnimationClip> animations)
    {
        var animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

        var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);
        clipOverrides["Idle"] = clipOverrides["Idle"];

        for (int i = 0; i < animations.Count; i++)
        {
            clipOverrides[animationKeys[i]] = animations[i];
        }

        animatorOverrideController.ApplyOverrides(clipOverrides);
        animator.runtimeAnimatorController = animatorOverrideController;
    }
}

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}