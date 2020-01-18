using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private const string kPerfStrandsLabel = "Hair Strands: ";
    private const string kPerfPhysicsBodiesLabel = "Physics Bodies: ";
    private const string kPerfFPSLabel = "FPS: ";

    private const int kDefaultHairLength = 30;
    private const int kDefaultRows = 6;
    private const float kDefaultSpacing = 5f;
    private const float kDefaultThickness = 0.5f;

    [Header("Hair Object")]
    public Hair hairReference;

    [Header("Sliders")]
    public Slider hairLengthSlider;
    public Slider rowCountSlider;
    public Slider spacingSlider;
    public Slider thicknessSlider;

    [Header("Toggles")]
    public Toggle randomizeHairLengthsToggle;

    [Header("Text Elements")]
    public Text perfStrandsText;
    public Text perfPhysicsBodiesText;
    public Text perfFPSText;

    private int hairLength;
    private float deltaTime = 0.0f;
    
    void Awake()
    {
        this.ResetDefaults();
    }

    public void OnHairLengthSliderChanged()
    {
        hairLengthSlider.GetComponentInChildren<Text>().text = hairLengthSlider.value.ToString();
    }
    public void OnRowSliderChanged()
    {
        rowCountSlider.GetComponentInChildren<Text>().text = rowCountSlider.value.ToString();
    }
    public void OnSpacingSliderChanged()
    {      
        spacingSlider.GetComponentInChildren<Text>().text = spacingSlider.value.ToString("0.0");
    }
    public void OnThicknessSliderChanged()
    {
        thicknessSlider.GetComponentInChildren<Text>().text = thicknessSlider.value.ToString("0.0");
    }

    public void OnRandomizeHairLengthsChanged(bool status)
    {
        hairReference.randomizeHairLengths = status;
    }

    public void ResetDefaults()
    {
        hairLengthSlider.value = kDefaultHairLength;
        rowCountSlider.value = kDefaultRows;
        spacingSlider.value = kDefaultSpacing;
        thicknessSlider.value = kDefaultThickness;
        UpdateHair();
    }

    public void UpdateHair()
    {
        hairReference.length = (int)hairLengthSlider.value;
        hairReference.numRows = (int)rowCountSlider.value;
        hairReference.strandSpacing = spacingSlider.value;
        hairReference.thickness = thicknessSlider.value;
        hairReference.Load();
        UpdatePerformanceStats();
    }

    void UpdatePerformanceStats()
    {
        perfStrandsText.text = kPerfStrandsLabel + (hairReference.numRows * hairReference.numRows);
        perfPhysicsBodiesText.text = kPerfPhysicsBodiesLabel + hairReference.bodyCount;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        UpdateFPSCounter();
    }

    void UpdateFPSCounter()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{1:0.} fps ({0:0.0} ms)", msec, fps);
        perfFPSText.text = kPerfFPSLabel + text;
    }
}
