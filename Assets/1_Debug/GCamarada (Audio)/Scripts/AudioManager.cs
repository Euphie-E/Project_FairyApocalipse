using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GCamarada
{
    public class AudioManager : MonoBehaviour
    {
        private List<AudioSource> musicSources = new List<AudioSource>();
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
        private Dictionary<string, int> sequentialIndices = new Dictionary<string, int>();

        public void PlaySequentialClip(string clipName)
        {
            foreach (var Audio in audioInfo)
            {
                if (Audio.clipName == clipName)
                {
                    if (!Audio.source)
                    {
                        Debug.LogWarning($"AudioSource não atribuído em: {clipName}");
                        return;
                    }

                    if (Audio.clip != null && Audio.clip.Length > 0)
                    {
                        if (!sequentialIndices.ContainsKey(clipName))
                            sequentialIndices[clipName] = 0;

                        int currentIndex = sequentialIndices[clipName];

                        if (Audio.clip[currentIndex] != null)
                        {
                            Audio.source.PlayOneShot(Audio.clip[currentIndex]);
                        }

                        sequentialIndices[clipName] = (currentIndex + 1) % Audio.clip.Length;
                    }
                    else
                    {
                        Debug.LogWarning($"A lista de 'clip' está vazia para: {clipName}");
                    }

                    break;
                }
            }
        }
        public void OnShotClip(string clipName)
        {
            foreach (var Audio in audioInfo)
            {
                if (Audio.clipName == clipName)
                {
                    if (!Audio.source)
                    {
                        Debug.LogWarning($"AudioSource não atribuído em: {clipName}");
                        return;
                    }


                    if (Audio.clipe != null)
                    {
                        Audio.source.PlayOneShot(Audio.clipe);
                    }

                    else if (Audio.clip != null && Audio.clip.Length > 0)
                    {
                        AudioClip randomClip = Audio.clip[Random.Range(0, Audio.clip.Length)];
                        if (randomClip != null)
                            Audio.source.PlayOneShot(randomClip);
                    }
                    else
                    {
                        Debug.LogWarning($"Nenhum AudioClip configurado para: {clipName}");
                    }

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
                    if (!Audio.source)
                    {
                        Debug.LogWarning($"AudioSource faltando em: {clipName}");
                        return;
                    }

                    AudioClip clipToPlay = null;

                    if (Audio.clipe != null)
                        clipToPlay = Audio.clipe;
                    else if (Audio.clip != null && Audio.clip.Length > 0)
                        clipToPlay = Audio.clip[Random.Range(0, Audio.clip.Length)];

                    if (clipToPlay == null)
                    {
                        Debug.LogWarning($"Nenhum AudioClip configurado para: {clipName}");
                        return;
                    }

                    Audio.source.clip = clipToPlay;
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
                    if (Audio.source != null)
                    {
                        Audio.source.Stop();
                        Audio.source.clip = null;
                    }
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
                Debug.LogWarning($"A variável audioMixer está sem valor!!");
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

        public void PlayFullMusic()
        {
            {
                foreach (var source in musicSources)
                {
                    source.Stop();
                }

                musicSources.Clear();

                foreach (var Audio in audioInfo)
                {
                    if (Audio.clipName == "fase1")
                    {
                        if (Audio.source == null)
                        {
                            Debug.LogWarning("AudioSource não encontrado na fase 1!");
                            return;
                        }

                        if (Audio.clip == null || Audio.clip.Length == 0)
                        {
                            Debug.LogWarning("Nenhuma música cadastrada na fase 1!");
                            return;
                        }

                        for (int i = 0; i < Audio.clip.Length; i++)
                        {
                            if (Audio.clip[i] == null)
                                continue;

                            AudioSource source;

                            if (i == 0)
                            {
                                source = Audio.source;
                            }
                            else
                            {
                                source = Audio.source.gameObject.AddComponent<AudioSource>();

                                source.outputAudioMixerGroup = Audio.source.outputAudioMixerGroup;
                                source.volume = Audio.source.volume;
                                source.pitch = Audio.source.pitch;
                            }

                            source.clip = Audio.clip[i];
                            source.loop = true;
                            source.spatialBlend = 0f;
                            source.Play();

                            musicSources.Add(source);
                        }

                        break;
                    }
                }
            }
        }

        public void StopMusic()
        {
            foreach (var source in musicSources)
            {
                if (source != null)
                {
                    source.Stop();
                }
            }

            musicSources.Clear();

            Debug.Log("Músicas da fase interrompidas.");
        }
    }


}