using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    public Light directionalLight;
    public float maxIntensity = 1.5f;
    public float dayHours = 16f;
    public float daySpeed = 1f;
    private float sunAngle;
    private float eastAngle;
    private float sunIntensity;
    private bool isDay = true;
    private float nightHours; // 60 grados = 1/3 día = 8hs -- 1h = 7.5

    void Start()
    {
        sunAngle = 35;
        eastAngle = 180; // Default 0
        nightHours = (24f - dayHours) * 7.5f;
        sunIntensity = maxIntensity;
        directionalLight.intensity = maxIntensity;
    }

    void Update()
    {
        //Util.DebugMe("nightHours", nightHours.ToString());
        //Util.DebugMe("sunIntensity", sunIntensity.ToString());

        if (isDay)
        {
            if (sunAngle < 180)
            {
                sunAngle += Time.deltaTime * daySpeed;
                // debugme.Show("SunAngle", sunAngle.ToString());
                directionalLight.transform.rotation = Quaternion.Euler(sunAngle, eastAngle, 0);
            } else
            {
                sunIntensity -= Time.deltaTime * daySpeed * (maxIntensity / 3.75f); // 0.5h amanecer 0.5h atardecer
                directionalLight.intensity = sunIntensity;
                if (sunIntensity < 0)
                {
                    directionalLight.GetComponent<Light>().enabled = false; // Find("DebugText").GetComponent<Text>()
                    isDay = false;
                    nightHours = (24f - dayHours) * 7.5f;
                }
            }
        }
        else
        {
            if (nightHours > 0)
            {
                nightHours -= Time.deltaTime * daySpeed;
                // falta transición "apagado" del cielo (Lighting > Ambient Color) / agregado estrellas y luna / y al revés antes de amanecer
            }
            else
            {
                if (sunIntensity < 0)
                {
                    sunAngle = 0;
                    directionalLight.transform.rotation = Quaternion.Euler(sunAngle, eastAngle, 0);
                    directionalLight.GetComponent<Light>().enabled = true; // directionalLight.gameObject.SetActive(true);
                    sunIntensity = 0;
                }
                if (sunIntensity <= maxIntensity)
                {
                    sunIntensity += Time.deltaTime * daySpeed * (maxIntensity / 3.75f); // 0.5h amanecer 0.5h atardecer
                    directionalLight.intensity = sunIntensity;
                }
                else
                {
                    sunIntensity = maxIntensity;
                    directionalLight.intensity = maxIntensity;
                    isDay = true;
                }
            }
        }
    }
}
