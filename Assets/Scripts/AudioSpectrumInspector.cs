// Audio spectrum component
// By Keijiro Takahashi, 2013
// https://github.com/keijiro/unity-audio-spectrum

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioSpectrum))]
public class AudioSpectrumInspector : Editor {
  #region Static definitions

  static string[] sampleOptionStrings = {
    "256", "512", "1024", "2048", "4096"
  };

  static int[] sampleOptions = {
    256, 512, 1024, 2048, 4096
  };

  static string[] bandOptionStrings = {
    "4 band", "4 band (visual)", "8 band", "10 band (ISO standard)", "26 band", "31 band (FBQ3102)"
  };

  static int[] bandOptions = {
    (int)AudioSpectrum.BandType.FourBand,
    (int)AudioSpectrum.BandType.FourBandVisual,
    (int)AudioSpectrum.BandType.EightBand,
    (int)AudioSpectrum.BandType.TenBand,
    (int)AudioSpectrum.BandType.TwentySixBand,
    (int)AudioSpectrum.BandType.ThirtyOneBand
  };

  #endregion

  #region Temporary state variables

  AnimationCurve curveLevels;
  AnimationCurve curvePeaks;
  AnimationCurve curveMeans;

  float curveLevelsHeight = 0;
  float curvePeaksHeight = 0;
  float curveMeansHeight = 0;

  #endregion

  #region Private functions

  void UpdateCurve() {
    var spectrum = target as AudioSpectrum;

    // Create a new curve to update the UI.
    curveLevels = new AnimationCurve();
    curveMeans = new AnimationCurve();
    curvePeaks = new AnimationCurve();

    var bands = spectrum.levels;
    // Add keys for the each band.
    for (var i = 0; i < bands.Length; i++) {
      curveLevels.AddKey(1.0f / bands.Length * i, spectrum.levels[i]);
      curveMeans.AddKey(1.0f / bands.Length * i, spectrum.meanLevels[i]);
      curvePeaks.AddKey(1.0f / bands.Length * i, spectrum.peakLevels[i]);
      curveLevelsHeight = Mathf.Max(curveLevelsHeight, spectrum.maxVolumeForLevel[i]);
      curvePeaksHeight = Mathf.Max(curvePeaksHeight, spectrum.maxVolumeForPeakLevel[i]);
      curveMeansHeight = Mathf.Max(curveMeansHeight, spectrum.maxVolumeForMeanLevel[i]);
    }
  }

  #endregion

  #region Editor callbacks

  public override void OnInspectorGUI() {
    var spectrum = target as AudioSpectrum;

    // Update the curve only when it's playing.
    if (EditorApplication.isPlaying) {
      UpdateCurve();
    } else if (curveLevels == null) {
      curveLevels = new AnimationCurve();
      curveMeans = new AnimationCurve();
      curvePeaks = new AnimationCurve();
    }

    // Component properties.
    spectrum.numberOfSamples = EditorGUILayout.IntPopup("Number of samples", spectrum.numberOfSamples,
      sampleOptionStrings, sampleOptions);
    spectrum.bandType =
      (AudioSpectrum.BandType)EditorGUILayout.IntPopup("Band type", (int)spectrum.bandType, bandOptionStrings,
        bandOptions);
    spectrum.fallSpeed = EditorGUILayout.Slider("Fall speed", spectrum.fallSpeed, 0.01f, 0.5f);
    spectrum.sensibility = EditorGUILayout.Slider("Sensibility", spectrum.sensibility, 1.0f, 20.0f);

    // Shows the spectrum curve.
    EditorGUILayout.CurveField(curveLevels, Color.white, new Rect(0, 0, 1.0f, 1), GUILayout.Height(64));
    EditorGUILayout.CurveField(curveMeans, Color.white, new Rect(0, 0, 1.0f, curveMeansHeight), GUILayout.Height(64));
    EditorGUILayout.CurveField(curvePeaks, Color.white, new Rect(0, 0, 1.0f, curvePeaksHeight), GUILayout.Height(64));

    // Update frequently while it's playing.
    if (EditorApplication.isPlaying) {
      EditorUtility.SetDirty(target);
    }
  }

  #endregion
}