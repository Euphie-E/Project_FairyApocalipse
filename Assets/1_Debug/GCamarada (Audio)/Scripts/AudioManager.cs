
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GCamarada
{
    public enum MusicTimeline
    {
        Both = 0,
        Future = 1,
        Past = 2
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager I;

        [Header("Audio Info")]
        [SerializeField]
        private List<AudioManagerList> audioInfo;

        [Header("References")]
        [SerializeField]
        private AudioMixer audioMixer;

        [Header("Time Travel")]
        [SerializeField]
        private TimeTravel timeTravel;

        // ================================
        // ESTADO TEMPORAL
        // ================================

        private bool isPast = false;

        // ================================
        // MÚSICAS
        // ================================

        private class MusicTrackRuntime
        {
            public AudioSource source;
            public MusicTimeline timeline;
        }

        private readonly List<MusicTrackRuntime> musicSources = new();

        // ================================
        // VFX
        // ================================

        // AudioSources dos OneShots e sequenciais
        private readonly Dictionary<int, AudioSource>
            oneShotSources = new();

        // AudioSources dos PlayClip e loops
        private readonly Dictionary<int, AudioSource>
            clipSources = new();

        // Índice sequencial de cada Audio Info
        private readonly Dictionary<int, int>
            sequentialIndices = new();

        // ================================
        // UNITY
        // ================================

        private void Awake()
        {
            if (I != null && I != this)
            {
                Destroy(gameObject);
                return;
            }

            I = this;
        }

        private void Start()
        {
            if (timeTravel != null)
            {
                UpdateMusicTimeline(
                    timeTravel.isTravelling
                );
            }
        }

        private void OnDestroy()
        {
            if (I != this)
                return;

            StopMusic();

            foreach (var source in oneShotSources.Values)
            {
                if (source != null)
                    Destroy(source);
            }

            foreach (var source in clipSources.Values)
            {
                if (source != null)
                    Destroy(source);
            }

            oneShotSources.Clear();
            clipSources.Clear();

            I = null;
        }

        // ================================
        // VERIFICAÇÃO TEMPORAL
        // ================================

        private bool CanPlayAudio(MusicTimeline timeline)
        {
            if (timeline == MusicTimeline.Both)
                return true;

            if (isPast)
                return timeline == MusicTimeline.Past;

            return timeline == MusicTimeline.Future;
        }

        public void UpdateMusicTimeline(bool past)
        {
            isPast = past;

            // Atualiza todas as músicas
            foreach (var track in musicSources)
            {
                if (track.source == null)
                    continue;

                track.source.mute =
                    !CanPlayAudio(track.timeline);
            }

            // Atualiza VFX OneShot
            foreach (var pair in oneShotSources)
            {
                int index = pair.Key;
                AudioSource source = pair.Value;

                if (source == null ||
                    index >= audioInfo.Count)
                    continue;

                source.mute =
                    !CanPlayAudio(audioInfo[index].timeline);
            }

            // Atualiza VFX PlayClip e loops
            foreach (var pair in clipSources)
            {
                int index = pair.Key;
                AudioSource source = pair.Value;

                if (source == null ||
                    index >= audioInfo.Count)
                    continue;

                source.mute =
                    !CanPlayAudio(audioInfo[index].timeline);
            }
        }

        // Alias opcional com nome mais genérico
        public void UpdateAudioTimeline(bool past)
        {
            UpdateMusicTimeline(past);
        }

        // ================================
        // CRIAÇÃO DE AUDIOSOURCES
        // ================================

        private AudioSource CreateRuntimeSource(
            AudioSource original)
        {
            if (original == null)
                return null;

            AudioSource source =
                original.gameObject.AddComponent<AudioSource>();

            source.playOnAwake = false;

            // Mixer e volume
            source.outputAudioMixerGroup =
                original.outputAudioMixerGroup;

            source.volume = original.volume;
            source.pitch = original.pitch;
            source.priority = original.priority;

            // Configurações espaciais
            source.spatialBlend = original.spatialBlend;
            source.panStereo = original.panStereo;
            source.spread = original.spread;
            source.dopplerLevel = original.dopplerLevel;
            source.minDistance = original.minDistance;
            source.maxDistance = original.maxDistance;
            source.rolloffMode = original.rolloffMode;

            source.spatialize = original.spatialize;

            // Configurações de efeitos
            source.bypassEffects = original.bypassEffects;
            source.bypassListenerEffects =
                original.bypassListenerEffects;

            source.bypassReverbZones =
                original.bypassReverbZones;

            source.reverbZoneMix =
                original.reverbZoneMix;

            if (original.rolloffMode ==
                AudioRolloffMode.Custom)
            {
                AnimationCurve curve =
                    original.GetCustomCurve(
                        AudioSourceCurveType.CustomRolloff
                    );

                if (curve != null)
                {
                    source.SetCustomCurve(
                        AudioSourceCurveType.CustomRolloff,
                        curve
                    );
                }
            }

            source.loop = false;
            source.mute = false;

            return source;
        }

        private AudioSource GetRuntimeSource(
            int index,
            AudioManagerList audio,
            Dictionary<int, AudioSource> dictionary)
        {
            if (dictionary.TryGetValue(index, out var source))
            {
                if (source != null)
                    return source;
            }

            if (audio.source == null)
            {
                Debug.LogWarning(
                    $"AudioSource não atribuído em: {audio.clipName}"
                );
                return null;
            }

            source = CreateRuntimeSource(audio.source);

            dictionary[index] = source;

            return source;
        }

        // ================================
        // SELEÇÃO DE AUDIOCLIP
        // ================================

        private AudioClip GetRandomClip(AudioManagerList audio)
        {
            if (audio.clipe != null)
                return audio.clipe;

            if (audio.clip == null ||
                audio.clip.Length == 0)
                return null;

            int start = Random.Range(0, audio.clip.Length);

            // Evita selecionar um índice vazio
            for (int i = 0; i < audio.clip.Length; i++)
            {
                int index =
                    (start + i) % audio.clip.Length;

                if (audio.clip[index] != null)
                    return audio.clip[index];
            }

            return null;
        }

        // ================================
        // VFX SEQUENCIAL
        // ================================

        public void PlaySequentialClip(string clipName)
        {
            for (int i = 0; i < audioInfo.Count; i++)
            {
                var audio = audioInfo[i];

                if (audio.clipName != clipName)
                    continue;

                if (!CanPlayAudio(audio.timeline))
                    continue;

                if (audio.clip == null ||
                    audio.clip.Length == 0)
                {
                    Debug.LogWarning(
                        $"Lista de clips vazia: {clipName}"
                    );
                    continue;
                }

                AudioSource source = GetRuntimeSource(
                    i,
                    audio,
                    oneShotSources
                );

                if (source == null)
                    return;

                if (!sequentialIndices.ContainsKey(i))
                    sequentialIndices[i] = 0;

                int currentIndex = sequentialIndices[i];

                // Procura o próximo clip válido
                for (int j = 0; j < audio.clip.Length; j++)
                {
                    int index =
                        (currentIndex + j) % audio.clip.Length;

                    if (audio.clip[index] != null)
                    {
                        source.mute = false;

                        source.PlayOneShot(
                            audio.clip[index]
                        );

                        sequentialIndices[i] =
                            (index + 1) % audio.clip.Length;

                        return;
                    }
                }

                Debug.LogWarning(
                    $"Nenhum clip válido: {clipName}"
                );

                return;
            }
        }

        // ================================
        // VFX ONESHOT
        // ================================

        public void OnShotClip(string clipName)
        {
            for (int i = 0; i < audioInfo.Count; i++)
            {
                var audio = audioInfo[i];

                if (audio.clipName != clipName)
                    continue;

                if (!CanPlayAudio(audio.timeline))
                    continue;

                AudioClip clipToPlay = GetRandomClip(audio);

                if (clipToPlay == null)
                {
                    Debug.LogWarning(
                        $"Nenhum AudioClip configurado: {clipName}"
                    );
                    continue;
                }

                AudioSource source = GetRuntimeSource(
                    i,
                    audio,
                    oneShotSources
                );

                if (source == null)
                    return;

                source.mute = false;
                source.PlayOneShot(clipToPlay);

                return;
            }
        }

        // ================================
        // PLAYCLIP E LOOPS
        // ================================

        public void PlayClip(string clipName, bool loop = false)
        {
            for (int i = 0; i < audioInfo.Count; i++)
            {
                var audio = audioInfo[i];

                if (audio.clipName != clipName)
                    continue;

                bool allowed =
                    CanPlayAudio(audio.timeline);

                // Sons normais não iniciam no período errado.
                if (!loop && !allowed)
                    continue;

                // Loops iniciam mesmo no período errado,
                // porém ficam mutados até o período correto.
                AudioClip clipToPlay = GetRandomClip(audio);

                if (clipToPlay == null)
                {
                    Debug.LogWarning(
                        $"Nenhum AudioClip configurado: {clipName}"
                    );
                    continue;
                }

                AudioSource source = GetRuntimeSource(
                    i,
                    audio,
                    clipSources
                );

                if (source == null)
                    continue;

                source.Stop();

                source.clip = clipToPlay;
                source.loop = loop;
                source.mute = !allowed;

                source.Play();

                // Para loops, inicia todas as entradas com
                // o mesmo nome (Future, Past e Both).
                // Para sons normais, utiliza a primeira válida.
                if (!loop)
                    return;
            }
        }

        // ================================
        // STOPCLIP
        // ================================

        public void StopClip(string clipName)
        {
            for (int i = 0; i < audioInfo.Count; i++)
            {
                var audio = audioInfo[i];

                if (audio.clipName != clipName)
                    continue;

                if (clipSources.TryGetValue(
                    i,
                    out AudioSource source))
                {
                    if (source != null)
                    {
                        source.Stop();
                        source.clip = null;
                        source.loop = false;
                    }
                }
            }
        }

        // ================================
        // MÚSICA COMPLETA
        // ================================

        public void PlayFullMusic()
        {
            PlayFullMusic("fase1");
        }

        public void PlayFullMusic(string musicName)
        {
            StopMusic();

            if (timeTravel != null)
                isPast = timeTravel.isTravelling;

            foreach (var audio in audioInfo)
            {
                if (audio.clipName != musicName)
                    continue;

                if (audio.source == null)
                {
                    Debug.LogWarning(
                        $"AudioSource faltando: {musicName}"
                    );
                    continue;
                }

                // Suporta array de clips ou um clipe único
                List<AudioClip> clipsToPlay = new();

                if (audio.clip != null &&
                    audio.clip.Length > 0)
                {
                    foreach (var clip in audio.clip)
                    {
                        if (clip != null)
                            clipsToPlay.Add(clip);
                    }
                }
                else if (audio.clipe != null)
                {
                    clipsToPlay.Add(audio.clipe);
                }

                foreach (var clip in clipsToPlay)
                {
                    AudioSource source =
                        CreateRuntimeSource(audio.source);

                    if (source == null)
                        continue;

                    source.clip = clip;
                    source.loop = true;
                    source.spatialBlend = 0f;

                    source.mute =
                        !CanPlayAudio(audio.timeline);

                    musicSources.Add(new MusicTrackRuntime
                    {
                        source = source,
                        timeline = audio.timeline
                    });
                }
            }

            if (musicSources.Count == 0)
            {
                Debug.LogWarning(
                    $"Nenhuma música encontrada: {musicName}"
                );
                return;
            }

            // Todas as faixas começam no mesmo instante
            double startTime =
                AudioSettings.dspTime + 0.15d;

            foreach (var track in musicSources)
            {
                if (track.source != null)
                {
                    track.source.PlayScheduled(startTime);
                }
            }
        }

        // ================================
        // STOP MUSIC
        // ================================

        public void StopMusic()
        {
            foreach (var track in musicSources)
            {
                if (track.source == null)
                    continue;

                track.source.Stop();

                Destroy(track.source);
            }

            musicSources.Clear();
        }

        // ================================
        // VOLUME / AUDIOMIXER
        // ================================

        public void SetVolumeValue(
            float value,
            string AudioMixName,
            TextMeshProUGUI volumeValueText)
        {
            if (volumeValueText != null)
                volumeValueText.text = value.ToString();

            if (audioMixer == null)
            {
                Debug.LogWarning(
                    "AudioMixer não atribuído!"
                );
                return;
            }

            float volume = Mathf.Clamp01(value / 100f);

            float decibeis =
                volume <= 0.001f
                ? -80f
                : Mathf.Log10(volume) * 20f;

            audioMixer.SetFloat(
                AudioMixName,
                decibeis
            );
        }

        public void SetMasterVolume(Slider slider)
        {
            if (slider == null)
                return;

            SetVolumeValue(
                slider.value,
                "AudioMaster",
                slider.GetComponentInChildren<TextMeshProUGUI>()
            );
        }

        public void SetVFXVolume(Slider slider)
        {
            if (slider == null)
                return;

            SetVolumeValue(
                slider.value,
                "AudioVFX",
                slider.GetComponentInChildren<TextMeshProUGUI>()
            );
        }

        public void SetMusicVolume(Slider slider)
        {
            if (slider == null)
                return;

            SetVolumeValue(
                slider.value,
                "AudioMusic",
                slider.GetComponentInChildren<TextMeshProUGUI>()
            );
        }
    }
}
