using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightMgr : MonoSingleton<DayNightMgr>
{
    public bool newSettings = true;
    public Light directionalLight;
    public Renderer sunMesh;
    public Camera mainCamera;
    public float maxIntensity = 2f;
    public Light spotLightRight;
    public Light spotLightLeft;
    public GTime dayDuration;
    public GTime nightDuration;
    public GTime sunsetDuration;
    public GTime sunriseDuration;
    public GTime newDayTime; // time of new day (after sunrise)
    public GTime newNightTime;
    public GTime newSunsetTime;
    public GTime newSunriseTime;
    public GTime dayRealTimeDuration;
    public GTime worldTime;
    // world objects starts looking at 0, Y rotation angles increase clockwise
    // directional light inverted: 0 = 180 world, 270 = 90 world (objects start looking at "west")
    public enum CardinalPoints : int
    {
        east = 0,
        ese = 22,
        southeast = 45,
        sse = 67,
        south = 90,
        ssw = 112,
        southwest = 135,
        wsw = 157,
        west = 180,
        wnw = 202,
        northwest = 225,
        nnw = 247,
        north = 270,
        nne = 292,
        northeast = 315,
        ene = 337
    }
    public CardinalPoints sunEast = CardinalPoints.east;
    public CardinalPoints sunWest = CardinalPoints.west;
    public enum DayPhases : int
    {
        sunrise,
        day,
        sunset,
        night
    }
    public DayPhases dayPhase;

    private float worldTimeSecs;
    private float secondsRTDay;
    private float secondsRTNight;
    private float secondsRTDayAdvance;
    private float secondsRTNightAdvance;
    private float dayAngleSpeed;
    private float nightAngleSpeed;
    public bool isDay;
    public float sunIntensity;
    public float sunAngle;
    public float nightHours;
    private Color sunColor;
    private Color ambienceColor;
    private Color skyColor;
    private byte tempAmbientColor;
    private float tempSkyColor;
    private Color tempColor;

    void Start()
    {
        //GetComponent<Renderer>().materials[0].mainTexture = null;
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        //playerRenderer.material.color = powerupExplodeOff;

        //Color tempColor = sunMesh.material.color;
        //Debug.Log(tempColor);
        //tempColor.a = 0;
        //sunMesh.material.color = tempColor;
        //Debug.Log(tempColor);

        InputMgr.OnSpotlightsClick += SetSpotlights;
        isDay = true;
        sunAngle = 90;
        sunIntensity = maxIntensity;
        directionalLight.intensity = maxIntensity;
        sunColor = sunMesh.material.color;
        ambienceColor = RenderSettings.ambientLight;
        skyColor = mainCamera.backgroundColor;
        dayDuration.CalculateTimeInSeconds();
        nightDuration.CalculateTimeInSeconds();
        sunsetDuration.CalculateTimeInSeconds();
        sunriseDuration.CalculateTimeInSeconds();
        dayRealTimeDuration.CalculateTimeInSeconds();
        CalculateSecondsRealTime();
        CalculateSunSpeed();
        // StartCoroutine(DayCycle());
    }

    void LateUpdate() // Old_DayNight_Routine()
    {
        // Use this if the player position moves fwd a lot and you need to keep sun disc size (Sun Mesh attached to directionalLight)
        directionalLight.transform.position = Vector3.forward * Control.playerPosition.z;

        if (isDay)
        {
            sunAngle += Time.deltaTime * dayAngleSpeed;
            directionalLight.transform.rotation = Quaternion.Euler(sunAngle, (float)sunEast, 0);

            if (sunAngle > 180) // Sunset
            {
                sunIntensity -= Time.deltaTime * dayAngleSpeed * (maxIntensity / 10f); // 3.75f); // 0.5h amanecer 0.5h atardecer
                directionalLight.intensity = sunIntensity;
                sunMesh.material.color = new Color32(0, 0, 0, (byte)(sunIntensity * 255 / maxIntensity));
                tempAmbientColor = (byte)(40 + (sunIntensity * 200 / maxIntensity));
                RenderSettings.ambientLight = new Color32(tempAmbientColor, tempAmbientColor, tempAmbientColor, 255);
                tempSkyColor = 0.59f - (sunIntensity * 0.59f / maxIntensity); // 0.59 ~ 150 / 0,55 ~ 140 byte
                tempColor = new Color(skyColor.r - tempSkyColor, skyColor.g - tempSkyColor, skyColor.b - tempSkyColor, skyColor.a);
                mainCamera.backgroundColor = tempColor;

                if (sunIntensity < 0)
                {
                    sunMesh.material.color = new Color32(0, 0, 0, 0);
                    isDay = false;
                    SetSpotlights(true);
                    nightHours = secondsRTNight;
                }
            }
        }
        else // Night
        {
            if (nightHours > 0)
            {
                nightHours -= Time.deltaTime * nightAngleSpeed;
                // TODO: agregado estrellas y luna
            }
            else // Sunrise
            {
                if (sunIntensity < 0)
                {
                    sunAngle = -10;
                    sunIntensity = 0;
                }
                if (sunIntensity <= maxIntensity)
                {
                    sunIntensity += Time.deltaTime * dayAngleSpeed * (maxIntensity / 35f);
                    directionalLight.intensity = sunIntensity;
                    sunMesh.material.color = new Color32(0, 0, 0, (byte)(sunIntensity * 255 / maxIntensity));
                    tempAmbientColor = (byte)(40 + (sunIntensity * 200 / maxIntensity));
                    RenderSettings.ambientLight = new Color32(tempAmbientColor, tempAmbientColor, tempAmbientColor, 255);
                    tempSkyColor = 0.59f - (sunIntensity * 0.59f / maxIntensity); // 0.59 ~ 150 / 0,55 ~ 140 byte
                    tempColor = new Color(skyColor.r - tempSkyColor, skyColor.g - tempSkyColor, skyColor.b - tempSkyColor, skyColor.a);
                    mainCamera.backgroundColor = tempColor;
                }
                else
                {
                    sunIntensity = maxIntensity;
                    directionalLight.intensity = maxIntensity;
                    sunMesh.material.color = sunColor;
                    RenderSettings.ambientLight = ambienceColor;
                    mainCamera.backgroundColor = skyColor;
                    isDay = true;
                    SetSpotlights(false);
                }
                sunAngle += Time.deltaTime * dayAngleSpeed;
                directionalLight.transform.rotation = Quaternion.Euler(sunAngle, (float)sunEast, 0);
            }
        }
    }

    private void SetSpotlights()
    {
        if (!isDay) SetSpotlights(!spotLightRight.enabled);
    }
    private void SetSpotlights(bool active)
    {
        spotLightRight.enabled = active;
        spotLightLeft.enabled = active;
    }

    private IEnumerator DayCycle()
    {
        // No termina nunca de funcionar correctamente. Temas a resolver:
        // - fracciones de segundo que se pierden al guardar float en worldtime integer. Creamos worldTimeSecs pero no la implementamos todavía.
        // - aun así, hay un desfasaje claro de horas en el avance del sol. Habría que buscar el bug y mejorar el reset diario que no está bien. Falta resetear worldtime también
        // - implementar cambios en cielo y sol, se puede tocar también la transparencia del material del sol para desaparecerlo
        // - es un desastre el avance a los saltos x segundo del sol en el cielo, aumentar frecuencia? (hay que cambiar cálculos...)
        // - a esta altura, parece convieniente basarse mejor en la versión vieja que trabaja sobre el Update fluído y fases del día atadas a ángulo del sol en lugar de horas del día
        while (true)
        {
            // Day  : 0-180 degrees
            // Night: 180-360 degrees (incl. sunrise + sunset)
            if (newSettings)
            {
                // For start and changing settings in real time
                directionalLight.transform.Rotate(Vector3.up * (int)sunEast); // TODO: Analyze how to rotate to no-opposite west (sunWest), e.g. extreme north/south regions sun cycle

                dayDuration.CalculateTimeInSeconds();
                nightDuration.CalculateTimeInSeconds();
                sunsetDuration.CalculateTimeInSeconds();
                sunriseDuration.CalculateTimeInSeconds();
                newDayTime.CalculateTimeInSeconds();
                newSunsetTime.TimeInSeconds = newDayTime.timeInSeconds + dayDuration.timeInSeconds;
                newNightTime.TimeInSeconds = newSunsetTime.timeInSeconds + sunsetDuration.timeInSeconds;
                newSunriseTime.TimeInSeconds = newNightTime.timeInSeconds + nightDuration.timeInSeconds;

                dayRealTimeDuration.CalculateTimeInSeconds();
                worldTime.CalculateTimeInSeconds();
                worldTimeSecs = worldTime.timeInSeconds;
                CalculateSecondsRealTime();
                CalculateSunSpeed();
                CalculateSunAngle();
                newSettings = false;
            }
            // Sun Position
            // Use this if the player position moves fwd a lot and you need to keep sun disc size (Sun Mesh attached to directionalLight)
            directionalLight.transform.position = Vector3.forward * Control.playerPosition.z;

            GetDayPhase();

            if (isDay) // dayPhase = DayPhases.day;
            {
                sunAngle += dayAngleSpeed;
                directionalLight.transform.Rotate(Vector3.right * dayAngleSpeed); // rotation = Quaternion.Euler(sunAngle, (float)sunEast, 0);
                worldTime.AddSeconds((int)secondsRTDayAdvance);
            }
            else
            {
                sunAngle += nightAngleSpeed;
                if (dayPhase == DayPhases.night) { }
                if (dayPhase == DayPhases.sunset)
                {
                    // sunIntensity = from maxIntensity to 0; + Lighting.AmbientColor.Intensite 0 to -2.5
                }
                if (dayPhase == DayPhases.sunrise)
                {
                    // sunIntensity = from 0 to maxIntensity; + Lighting.AmbientColor.Intensite -2.5 to 0
                }
                directionalLight.transform.Rotate(Vector3.right * nightAngleSpeed); // rotation = Quaternion.Euler(sunAngle, (float)sunEast, 0);
                worldTime.AddSeconds((int)secondsRTNightAdvance);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void GetDayPhase()
    {
        // Set dayPhase and specific settings for each one
        if (worldTime.timeInSeconds >= newDayTime.timeInSeconds && worldTime.timeInSeconds < newSunsetTime.timeInSeconds && dayPhase != DayPhases.day)
        {
            dayPhase = DayPhases.day;
            sunAngle = 0;
            isDay = true;
            directionalLight.intensity = maxIntensity;
            // TODO: Generar evento para que karting apague luces (agregar luces, seguramente > también enemigos)
        }
        else if ((worldTime.timeInSeconds >= newNightTime.timeInSeconds || worldTime.timeInSeconds < newSunriseTime.timeInSeconds) && dayPhase != DayPhases.night)
        {
            dayPhase = DayPhases.night;
            isDay = false;
        }
        else if (worldTime.timeInSeconds >= newSunsetTime.timeInSeconds && worldTime.timeInSeconds < newNightTime.timeInSeconds && dayPhase != DayPhases.sunset)
        {
            dayPhase = DayPhases.sunset;
            isDay = false;
            // sunIntensity = from maxIntensity to 0; + Lighting.AmbientColor.Intensite 0 to -2.5
            // TODO: Generar evento para que karting encienda luces (agregar luces, seguramente > también enemigos)
        }
        else if (worldTime.timeInSeconds >= newSunriseTime.timeInSeconds && worldTime.timeInSeconds < newDayTime.timeInSeconds && dayPhase != DayPhases.sunrise)
        {
            dayPhase = DayPhases.sunrise;
            isDay = false;
            // sunIntensity = from 0 to maxIntensity; + Lighting.AmbientColor.Intensite -2.5 to 0
        }
    }

    private void CalculateSecondsRealTime()
    {
        // Day and Night have different duration. Seconds real time is calculated for both.
        secondsRTDay = dayRealTimeDuration.timeInSeconds * dayDuration.timeInSeconds / (24 * 3600);
        secondsRTNight = dayRealTimeDuration.timeInSeconds * (nightDuration.timeInSeconds + sunriseDuration.timeInSeconds + sunsetDuration.timeInSeconds) / (24 * 3600);
        secondsRTDayAdvance = (24 * 3600) / secondsRTDay;
        secondsRTNightAdvance = (24 * 3600) / secondsRTNight;
    }
    private void CalculateSunSpeed()
    {
        // Calculate Sun angle velocity per second (different for day and night)
        dayAngleSpeed = 180 / secondsRTDay;
        nightAngleSpeed = 180 / secondsRTNight;
    }
    private void CalculateSunAngle()
    {
        // Calculate Sun Position for a given world time:
        if (isDay)
        {
            sunAngle = (worldTime.timeInSeconds - newDayTime.timeInSeconds) * secondsRTDay / dayDuration.timeInSeconds * dayAngleSpeed;
        }
        else
        {
            if (worldTime.hours < 24)
            {
                sunAngle = (worldTime.timeInSeconds - (newDayTime.timeInSeconds + dayDuration.timeInSeconds)) * secondsRTNight / (nightDuration.timeInSeconds + sunriseDuration.timeInSeconds + sunsetDuration.timeInSeconds)
                    * dayAngleSpeed + 180;
            }
            else
            {
                sunAngle = ((worldTime.timeInSeconds + (24 * 3600)) - (newDayTime.timeInSeconds + dayDuration.timeInSeconds)) * secondsRTNight / (nightDuration.timeInSeconds + sunriseDuration.timeInSeconds + sunsetDuration.timeInSeconds)
                    * dayAngleSpeed + 180;
            }
        }
        directionalLight.transform.Rotate(Vector3.right * sunAngle);
    }
}