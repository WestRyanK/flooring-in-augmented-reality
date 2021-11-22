using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSettings : MonoBehaviour
{
    [SerializeField]
    private bool AllowLandscape;
    [SerializeField]
    private bool AllowPortrait;
    [SerializeField]
    private bool PreventSleep;

    void Start()
    {
        Screen.autorotateToLandscapeLeft = AllowLandscape;
        Screen.autorotateToLandscapeRight = AllowLandscape;
        Screen.autorotateToPortrait = AllowPortrait;
        Screen.autorotateToPortraitUpsideDown = false;
        
        if (PreventSleep)
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}
		else 
        { 
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
}
