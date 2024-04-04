using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CursedGate : Gate
{
    public float colorFadeTime;
    public float tempFadeTime;

    public Color cursedLanternColor;
    public int cursedLanternTemp;

    public float upgradeLanternTimeDelay;
    public float lanternBuffMultiplier;

    public float textfadeTime;
    public float textFadeOutDelay;
    public TextMeshProUGUI toolTipTextObj;



    
    public override IEnumerator Closegate()
    {
        Light[] lanternLights = PlayerController.Instance.GetComponentsInChildren<Light>();

        StartCoroutine(UpgradeLanternDelay(lanternLights[0]));
        StartCoroutine(ChangePlayerLanternColor(lanternLights));

        return base.Closegate();
    }

    private IEnumerator ChangePlayerLanternColor(Light[] lanternLights)
    {
        float tempDist = cursedLanternTemp - lanternLights[0].colorTemperature;

        while (lanternLights[0].color != cursedLanternColor && lanternLights[0].colorTemperature != cursedLanternTemp)
        {
            foreach (Light light in lanternLights)
            {
                light.color = Color.Lerp(light.color, cursedLanternColor, 1f / colorFadeTime * Time.deltaTime);
                light.colorTemperature = Mathf.MoveTowards(light.colorTemperature, cursedLanternTemp, tempDist / tempFadeTime * Time.deltaTime);
            }
            yield return null;
        }
    }
    private IEnumerator UpgradeLanternDelay(Light lanternLight)
    {
        yield return new WaitForSeconds(upgradeLanternTimeDelay);
        lanternLight.transform.parent.GetChild(2).gameObject.SetActive(true);

        lanternLight.intensity *= lanternBuffMultiplier;
        lanternLight.range *= lanternBuffMultiplier;


        toolTipTextObj.gameObject.SetActive(true);

        while (toolTipTextObj.color.a < 1)
        {
            toolTipTextObj.color += new Color(0, 0, 0, 1 / colorFadeTime * Time.deltaTime);
            yield return null;
        }


        yield return new WaitForSeconds(textFadeOutDelay);

        while (toolTipTextObj.color.a > 0)
        {
            toolTipTextObj.color -= new Color(0, 0, 0, 1 / colorFadeTime * Time.deltaTime);
            yield return null;
        }
        toolTipTextObj.gameObject.SetActive(false);
    }
}
