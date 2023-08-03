using UnityEngine;

public class AndroidPropertiesManager: MonoBehaviour
{
    private void Awake()
    {
#if PLATFORM_ANDROID
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
    }
}
