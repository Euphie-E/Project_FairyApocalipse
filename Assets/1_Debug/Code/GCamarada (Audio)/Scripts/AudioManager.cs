using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace GCamarada
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager I;
        
        [SerializeField] private List<AudioManagerList> audioInfo;
        
        [Header("References")]
        
        [SerializeField] private AudioMixer audioMixer;

        private void Awake()
        {
            if (I == null)
                I = this;
            else 
                Destroy(gameObject);
        }
        public void OnShotClip(string clipName)
        {
            foreach (var Audio in audioInfo)
            {
                if (Audio.clipName == clipName)
                {
                    if (!Audio.clip || !Audio.source)
                        Debug.LogWarning($"Verifique a lista de audios pois algo esta faltando!! Audio Name: {Audio.clipName}");

                    Audio.source.PlayOneShot(Audio.clip);
                    break;
                }
            }
        }
        public void PlayClip(string clipName, bool loop = false)
        {
            foreach (var Audio in audioInfo)
            {
                if (Audio.clipName == clipName)
                {
                    if (!Audio.clip || !Audio.source)
                        Debug.LogWarning($"Verifique a lista de audios pois algo esta faltando!! Audio Name: {Audio.clipName}");

                    Audio.source.clip = Audio.clip;
                    Audio.source.loop = loop;
                    Audio.source.Play();

                    break;
                }
            }
        }
        public void StopClip(string clipName)
        {
            foreach (var Audio in audioInfo)
            {
                if (Audio.clipName == clipName)
                {
                    if (!Audio.clip || !Audio.source)
                        Debug.LogWarning($"Verifique a lista de audios pois algo esta faltando!! Audio Name: {Audio.clipName}");
                    
                    if (Audio.source.clip)
                        Audio.source.Stop();

                    break;
                }
            }
        }

        public void SetVolumeValue(float value, string AudioMixName, TextMeshProUGUI volumeValueText)
        {
            if (volumeValueText)
                volumeValueText.text = value.ToString();
 
            if (!audioMixer)
            {
                Debug.LogWarning($"A variavel audioMixer esta sem valor!!");
                return;
            }

            float volume = value / 100f;
            float decibeis = Mathf.Log10(volume) * 20f;
            audioMixer.SetFloat(AudioMixName, volume <= .001f ? -80 : decibeis);
        }

        public void SetMasterVolume(Slider slider)
        {
            SetVolumeValue(slider.value, "AudioMaster", slider.GetComponentInChildren<TextMeshProUGUI>());
        }
        public void SetVFXVolume(Slider slider)
        {
            SetVolumeValue(slider.value, "AudioVFX", slider.GetComponentInChildren<TextMeshProUGUI>());
        }
        public void SetMusicVolume(Slider slider)
        {
            SetVolumeValue(slider.value, "AudioMusic", slider.GetComponentInChildren<TextMeshProUGUI>());
        }
    }
}