using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        SetColour(Clear);
    }

    public void OnPowerChanged()
    {
        if (power.FullPowerup() && power.AllPowerupsAreDifferentTypes())
        {
            SetColour(RainbowMode);
        }
        else
        {
            Gradient baseColour = GetGradient(power.Powerups[0], 0);
            Gradient primaryDetailColour = GetGradient(power.Powerups[1], 1);
            Gradient secondaryDetailColour = GetGradient(power.Powerups[2], 2);
            SetColour(baseColour, primaryDetailColour, secondaryDetailColour);
        }
    }

    private void SetColour(ColourSet set)
    {
        SetColour(set.Base, set.PrimaryDetails, set.SecondaryDetails);
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
