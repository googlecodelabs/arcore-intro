using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodelabUtils
{
  
    /// <summary>Coroutine to display an error then exit.</summary>
    public static IEnumerator ToastAndExit(string message, int seconds)
    {
        _ShowAndroidToastMessage(message);
        yield return new WaitForSeconds(seconds);
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    /// <param name="length">Toast message time length.</param>
    public static void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass =
                new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                    {
                        AndroidJavaObject toastObject =
                            toastClass.CallStatic<AndroidJavaObject>(
                                "makeText", unityActivity,
                                message, 0);
                        toastObject.Call("show");
                    }));
        }
    }
}
