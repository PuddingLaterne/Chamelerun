using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChameleonColour : MonoBehaviour
{
    [System.Serializable]
    public struct ColourSet
    {
        public Gradient Base;
        public Gradient PrimaryDetails;
        public Gradient SecondaryDetails;
    }

    [System.Serializable]
    public struct Gradient
    {
        public Color Shadows;
        public Color Midtones;
        public Color Highlights;
    }

    public float FadeDuration = 0.5f;
    public float FadeSteps = 8;

    public ColourSet Clear;

    public ColourSet Red;
    public ColourSet Yellow;
    public ColourSet Blue;

    public ColourSet RainbowMode;

    private Material material;
    private ChameleonPower power;

    private int mainShadowsID;
    private int mainMidtonesID;
    private int mainHighlightsID;

    private int primaryShadowsID;
    private int primaryMidtonesID;
    private int primaryHighlightsID;

    private int secondaryShadowsID;
    private int secondaryMidtonesID;
    private int secondaryHighlightsID;

    public void Init(ChameleonPower power)
    {
        this.power = power;
        Renderer rend = GetComponent<Renderer>();
        material = rend.sharedMaterial;

        mainShadowsID = Shader.PropertyToID("_MainShadows");
        mainMidtonesID = Shader.PropertyToID("_MainMidtones");
        mainHighlightsID = Shader.PropertyToID("_MainHighlights");

        primaryShadowsID = Shader.PropertyToID("_DetailAShadows");
        primaryMidtonesID = Shader.PropertyToID("_DetailAMidtones");
        primaryHighlightsID = Shader.PropertyToID("_DetailAHighlights");

        secondaryShadowsID = Shader.PropertyToID("_DetailBShadows");
        secondaryMidtonesID = Shader.PropertyToID("_DetailBMidtones");
        secondaryHighlightsID = Shader.PropertyToID("_DetailBHighlights");
    }

    public void Reset()
    {
        StopAllCoroutines();
        SetColour(Clear.Base, Clear.PrimaryDetails, Clear.SecondaryDetails);
    }

    public void OnPowerChanged()
    {
        StopAllCoroutines();
        if (power.FullPowerup() && power.AllPowerupsAreDifferentTypes())
        {
            StartCoroutine(SetColourGradually(RainbowMode.Base, RainbowMode.PrimaryDetails, RainbowMode.PrimaryDetails));
        }
        else
        {
            Gradient baseColour = GetGradient(power.Powerups[0], 0);
            Gradient primaryDetailColour = GetGradient(power.Powerups[1], 1);
            Gradient secondaryDetailColour = GetGradient(power.Powerups[2], 2);
            StartCoroutine(SetColourGradually(baseColour, primaryDetailColour, secondaryDetailColour));
        }
    }

    private void SetColour(Gradient baseColour, Gradient primaryDetailColour, Gradient secondaryDetailColour)
    {
        material.SetColor(mainShadowsID, baseColour.Shadows);
        material.SetColor(mainMidtonesID, baseColour.Midtones);
        material.SetColor(mainHighlightsID, baseColour.Highlights);

        material.SetColor(primaryShadowsID, primaryDetailColour.Shadows);
        material.SetColor(primaryMidtonesID, primaryDetailColour.Midtones);
        material.SetColor(primaryHighlightsID, primaryDetailColour.Highlights);

        material.SetColor(secondaryShadowsID, secondaryDetailColour.Shadows);
        material.SetColor(secondaryMidtonesID, secondaryDetailColour.Midtones);
        material.SetColor(secondaryHighlightsID, secondaryDetailColour.Highlights);
    }
    
    private IEnumerator SetColourGradually(Gradient baseColour, Gradient primaryDetailColour, Gradient secondaryDetailColour)
    {
        float fadeStepDuration = FadeDuration / FadeSteps;
        float fadeStep = 1f / FadeSteps;

        Color baseShadowsOld = material.GetColor(mainShadowsID);
        Color baseMidtonesOld = material.GetColor(mainMidtonesID);
        Color baseHighlightsOld = material.GetColor(mainHighlightsID);

        Color primaryDetailShadowsOld = material.GetColor(primaryShadowsID);
        Color primaryDetailMidtonesOld = material.GetColor(primaryMidtonesID);
        Color primaryDetailHighlightsOld = material.GetColor(primaryHighlightsID);

        Color secondaryDetailShadowsOld = material.GetColor(secondaryShadowsID);
        Color secondaryDetailMidtonesOld = material.GetColor(secondaryMidtonesID);
        Color secondaryDetailHighlightsOld = material.GetColor(secondaryHighlightsID);

        for (int i = 1; i <= FadeSteps; i++)
        {
            yield return new WaitForSeconds(fadeStepDuration);
            material.SetColor(mainShadowsID, Color.Lerp(baseShadowsOld, baseColour.Shadows, fadeStep * i));
            material.SetColor(mainMidtonesID, Color.Lerp(baseMidtonesOld, baseColour.Midtones, fadeStep * i));
            material.SetColor(mainHighlightsID, Color.Lerp(baseHighlightsOld, baseColour.Highlights, fadeStep * i));

            material.SetColor(primaryShadowsID, Color.Lerp(primaryDetailShadowsOld, primaryDetailColour.Shadows, fadeStep * i));
            material.SetColor(primaryMidtonesID, Color.Lerp(primaryDetailMidtonesOld, primaryDetailColour.Midtones, fadeStep * i));
            material.SetColor(primaryHighlightsID, Color.Lerp(primaryDetailHighlightsOld, primaryDetailColour.Highlights, fadeStep * i));

            material.SetColor(secondaryShadowsID, Color.Lerp(secondaryDetailShadowsOld, secondaryDetailColour.Shadows, fadeStep * i));
            material.SetColor(secondaryMidtonesID, Color.Lerp(secondaryDetailMidtonesOld, secondaryDetailColour.Midtones, fadeStep * i));
            material.SetColor(secondaryHighlightsID, Color.Lerp(secondaryDetailHighlightsOld, secondaryDetailColour.Highlights, fadeStep * i));
        }
    }

    private Gradient GetGradient(PowerupType powerupType, int colourIndex)
    {
        switch(powerupType)
        {
            case PowerupType.Clear:
                return GetGradientFromColourSet(Clear, colourIndex);
            case PowerupType.Red:
                return GetGradientFromColourSet(Red, colourIndex);
            case PowerupType.Yellow:
                return GetGradientFromColourSet(Yellow, colourIndex);
            case PowerupType.Blue:
                return GetGradientFromColourSet(Blue, colourIndex);
            default:
                return GetGradientFromColourSet(Clear, colourIndex);
        }
    }

    private Gradient GetGradientFromColourSet(ColourSet set, int colourIndex)
    {
        switch(colourIndex)
        {
            case 0:
                return set.Base;
            case 1:
                return set.PrimaryDetails;
            case 2:
                return set.SecondaryDetails;
            default:
                return set.Base;
        }
    }
}
