using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioLoudnessDetector : MonoBehaviour {
  private int _loudnessSampleWindow = 64;
  private int _sampleSize = 512;
  private int _bandSize = 8;
  private float[] _bandBuffer;
  private float[] _bufferDecrease;
  private float[] _samples;
  private float[] _freqBands;

  private float[] _maxAudioPerBand;
  public float[] audioBand;
  public float[] audioBandBuffer;

  public float amplitude;
  public float amplitudeBuffer;
  private float _amplitudeMax;


  private AudioClip _microphoneClip;
  private AudioSource _source;
  private AudioListener _listener;

  void Start() {
    _samples = new float[_sampleSize];

    _freqBands = new float[_bandSize];
    _bandBuffer = new float[_bandSize];
    _bufferDecrease = new float[_bandSize];
    _maxAudioPerBand = new float[_bandSize];
    audioBand = new float[_bandSize];
    audioBandBuffer = new float[_bandSize];


    _source = GetComponent<AudioSource>();
    _source.Stop();
    _source.loop = true;

    string microphoneName = Microphone.devices[0];

    _source.clip = Microphone.Start(microphoneName, true, 10, AudioSettings.outputSampleRate);
    if (Microphone.IsRecording(microphoneName)) {
      while (!(Microphone.GetPosition(microphoneName) > 0)) {
        // Wait for the microphone to start recording...
      }

      _source.Play();
    } else {
      Debug.Log("Microphone not working as expected: " + microphoneName);
    }
  }

  void UpdateBandBuffer() {
    for (int i = 0; i < _bandSize; i++) {
      if (_bandBuffer[i] > _freqBands[i]) {
        _bandBuffer[i] = Mathf.Max(0, _bandBuffer[i] - _bufferDecrease[i]);
        _bufferDecrease[i] *= 1.2f;
      } else {
        _bandBuffer[i] = _freqBands[i];
        _bufferDecrease[i] = 0.005f;
      }
    }
  }


  void UpdateMaxPerBand() {
    for (int i = 0; i < _bandSize; i++) {
      _maxAudioPerBand[i] = Mathf.Max(_maxAudioPerBand[i], _freqBands[i]);
      audioBand[i] = _freqBands[i] / _maxAudioPerBand[i];
      audioBandBuffer[i] = _bandBuffer[i] / _maxAudioPerBand[i];
    }
  }

  private void FixedUpdate() {
    UpdateWaveData();
    UpdateBandBuffer();
    UpdateMaxPerBand();
    UpdateAmplitude();
  }

  void UpdateAmplitude() {
    float curAmp = 0;
    float curAmpBuffer = 0;

    for (int i = 0; i < _bandSize; i++) {
      curAmp += audioBand[i];
      curAmpBuffer += audioBandBuffer[i];
    }

    _amplitudeMax = Mathf.Max(_amplitudeMax, curAmp);
    amplitude = curAmp / _amplitudeMax;
    amplitudeBuffer = curAmpBuffer / _amplitudeMax;
  }

  // public float GetLoudnessFromMicrophone() {
  //   return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[0]), _microphoneClip);
  // }
  //
  // public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip) {
  //   int startPosition = clipPosition - _loudnessSampleWindow;
  //
  //   if (startPosition < 0) {
  //     return 0;
  //   }
  //
  //   float[] waveDate = new float[_loudnessSampleWindow];
  //   _source.clip.GetData(waveDate, startPosition);
  //
  //   float totalLoudness = 0;
  //
  //   for (int i = 0; i < _loudnessSampleWindow; i++) {
  //     totalLoudness += Mathf.Abs(waveDate[i]);
  //   }
  //
  //   return totalLoudness / _loudnessSampleWindow;
  // }

  public void UpdateWaveData() {
    _source.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);

    var chunk = _sampleSize / _bandSize;

    for (int i = 0; i < _bandSize; i++)
    {
      _freqBands[i] = _samples.Skip(i * chunk).Take(chunk).Average();
    }

    
    //
    //
    // int count = 0;
    // for (int i = 0; i < _bandSize; i++) {
    //   float avg = 0;
    //   int sampleCount = (int)Mathf.Pow(2, i) * 2;
    //   for (int j = 0; j < sampleCount; j++) {
    //     avg += _samples[count] * (count + 1);
    //     count++;
    //   }
    //
    //   avg /= count;
    //
    //   _freqBands[i] = avg * 10;
      // _bands[i] = _spectrumData.Skip(i * chunk).Take(chunk).Average();
    // }
  }
}