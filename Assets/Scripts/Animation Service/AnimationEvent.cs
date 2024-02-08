using UnityEngine;
using System; 

public class AnimationEvent : MonoBehaviour
{
    public event Action OnExerciseCompleted;
    public event Action OnProgramCompleted; 

    // This is the animation event, defined/called by animation
    public void ProgramComplete()
    {
        OnProgramCompleted?.Invoke();
    }
    public void ExerciseComplete()
    {
        OnExerciseCompleted?.Invoke();
    } 
}
