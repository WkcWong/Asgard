using UnityEngine;
using System.Collections;

public class Fading : MonoBehaviour
{
    public Texture2D fadeTexture;
    private float fadeSpeed = 0.8f;
    private int drawDepth = -1000;

    private float alpha = 1f;
    private float fadeDir = -1;

    void OnGUI()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        GUI.DrawTexture(new Rect(0, 0, 100000, 100000), fadeTexture);
    }

    public float BeginFade(int direction)
    {
        fadeDir = direction;
        return (fadeSpeed);
    }
}