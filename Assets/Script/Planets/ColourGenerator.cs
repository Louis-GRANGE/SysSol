﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator
{
    public ColourSettings settings { get; private set; }
    private Texture2D texture;
    private const int textureResolution = 50;
    private INoiseFilter biomeNoiseFilter;

    public void UpdateSettings(ColourSettings settings)
    {
        this.settings = settings;
        if (texture == null || texture.height != settings.biomeColourSettings.biomes.Length)
        {
            texture = new Texture2D(textureResolution * 2, settings.biomeColourSettings.biomes.Length, TextureFormat.RGBA32, false);
        }
        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColourSettings.noise);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColourSettings.noiseOffset) *
                         settings.biomeColourSettings.noiseStrength;
        float biomeIndex = 0;
        int numBiomes = settings.biomeColourSettings.biomes.Length;
        float blendRange = settings.biomeColourSettings.blendAmount / 2f + .001f;

        for (int i = 0; i < numBiomes; i++)
        {
            float distance = heightPercent - settings.biomeColourSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, distance);
            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;
        }
        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }
    
    public void UpdateColours()
    {
        Color[] colours = new Color[texture.width * texture.height];
        int colorIndex = 0;
        foreach (var biome in settings.biomeColourSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; i++)
            {
                Color gradientColor;
                if (i < textureResolution)
                {
                    gradientColor = settings.oceanColour.Evaluate(i / (textureResolution - 1f));
                }
                else
                {
                    gradientColor = biome.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f));
                }
                Color tintColor = biome.tint;
                colours[colorIndex] = gradientColor * (1 - biome.tintPercent) + tintColor * biome.tintPercent;
                colorIndex++;
            }
        }

        texture.SetPixels(colours);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture);
    }
}
