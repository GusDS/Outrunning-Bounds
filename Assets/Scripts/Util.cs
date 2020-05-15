using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Util
{
    private static Text debugText = GameObject.Find("DebugText").GetComponent<Text>();
    private static Dictionary<string, string> watches = new Dictionary<string, string>();

    static Util() {}

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