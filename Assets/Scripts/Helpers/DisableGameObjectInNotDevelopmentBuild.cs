 
using UnityEngine;

public class DisableGameObjectInNotDevelopmentBuild : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD  
#else
        gameObject.SetActive(false);
#endif 
    } 
}