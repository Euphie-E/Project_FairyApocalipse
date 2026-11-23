using System;
using UnityEngine;

namespace GCamarada
{
    [Serializable]
    public class AudioManagerList 
    {
        public string clipName;
        public AudioClip clipe;
        public AudioClip[] clip;
        public AudioSource source;
        [Header("Tempo do audio")]
        public MusicTimeline timeline = MusicTimeline.Both;
    }
}

