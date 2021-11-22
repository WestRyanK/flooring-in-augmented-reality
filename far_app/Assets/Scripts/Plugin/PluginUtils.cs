using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PluginUtils
{
    private static Dictionary<string, AndroidJavaClass> javaClassInstances = new Dictionary<string, AndroidJavaClass>();
    private static AndroidJavaObject context;

    private static AndroidJavaClass GetJavaClass(string javaClassName)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.Log($"Cannot call Android methods from platform '{Application.platform}'!");
            return null;
        }

        bool foundCachedValue = javaClassInstances.TryGetValue(javaClassName, out AndroidJavaClass javaClass);
        if (!foundCachedValue)
        {
            javaClass = new AndroidJavaClass(javaClassName);
            javaClassInstances.Add(javaClassName, javaClass);
        }
        return javaClass;
    }

    public static T StaticCall<T>(string methodName, string javaClassName, params object[] args)
    {
        AndroidJavaClass javaClass = GetJavaClass(javaClassName);
        AndroidJNI.AttachCurrentThread();
        return javaClass.CallStatic<T>(methodName, args);
    }

    public static void StaticCall(string methodName, string javaClassName, params object[] args)
    {
        AndroidJavaClass javaClass = GetJavaClass(javaClassName);
        AndroidJNI.AttachCurrentThread();
        javaClass.CallStatic(methodName, args);
    }

    public static AndroidJavaObject GetAndroidContext()
    {
        if (PluginUtils.context == null)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
            PluginUtils.context = context;
        }

        return PluginUtils.context;
    }
}
