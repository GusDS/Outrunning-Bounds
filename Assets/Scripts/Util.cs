using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Util
{
    private static Text debugText = GameObject.Find("DebugText").GetComponent<Text>();
    private static Dictionary<string, string> watches = new Dictionary<string, string>();

    static Util() {}

    public static void DebugMe(string text, float value) { DebugMe(text, value.ToString()); }
    public static void DebugMe(string text, int value) { DebugMe(text, value.ToString()); }
    public static void DebugMe(string text, byte value) { DebugMe(text, value.ToString()); }
    public static void DebugMe(string text, Color32 value) { DebugMe(text, value.ToString()); }
    public static void DebugMe(string text, string value)
    {
        string watchText = string.Empty;
        if (watches.ContainsKey(text)) watches[text] = value;
        else watches.Add(text, value);

        foreach (KeyValuePair<string, string> dicItem in watches) {
            watchText += dicItem.Key + ":\t\t\t" + dicItem.Value + "\n";
        }
        debugText.text = watchText;
    }
}

[System.Serializable]
public class GTime
{
    public int hours;
    public int minutes;
    public int seconds;
    public int timeInSeconds;

    public int Hours {
        get { return hours; }
        set {
            hours = value;
            CalculateTimeInSeconds();
        }
    }
    public int Minutes {
        get { return minutes; }
        set {
            minutes = value;
            CalculateTimeInSeconds();
        }
    }
    public int Seconds {
        get { return seconds; }
        set {
            seconds = value;
            CalculateTimeInSeconds();
        }
    }
    public int TimeInSeconds {
        get { return timeInSeconds; }
        set {
            timeInSeconds = value;
            CalculateTimeInHours();
        }
    }
    public void CalculateTimeInSeconds() {
        timeInSeconds = (hours * 3600) + (minutes * 60) + seconds;
    }
    public void CalculateTimeInHours() {
        int secs = timeInSeconds;
        hours = secs / 3600;
        secs %= 3600;
        minutes = secs / 60;
        secs %= 60;
        seconds = secs;
        if (hours > 23) {
            hours -= 24;
            CalculateTimeInSeconds();
        }
    }
    public void AddSeconds(int secs = 1) {
        timeInSeconds += secs;
        CalculateTimeInHours();
    }
}