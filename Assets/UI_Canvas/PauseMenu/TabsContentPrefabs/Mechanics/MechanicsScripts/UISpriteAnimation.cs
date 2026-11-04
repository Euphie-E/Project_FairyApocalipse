using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    public Image m_Image;

    public Sprite[] m_SpriteArray;
    public float m_Speed = .04f;

    public bool Loop = true;

    public AudioSource SistemaAudio;
    public AudioClip Audio;

    private int m_IndexSprite;
    private Coroutine m_CorotineAnim;

    public bool IsDone, IsPlaying;

    private void Awake()
    {
        if (m_Image == null)
            m_Image = GetComponent<Image>();
    }

    public void Play()
    {
        Stop();

        if (m_Image == null || m_SpriteArray == null || m_SpriteArray.Length == 0)
        {
            Debug.LogWarning("UISpriteAnimation.Play: imagem ou array de sprites inválido.");
            return;
        }

        IsDone = false;
        m_IndexSprite = 0;
        m_Image.sprite = m_SpriteArray[0];
        m_Image.enabled = true;
        IsPlaying = true;
        m_CorotineAnim = StartCoroutine(Func_PlayAnimUI());

        if (Audio != null && SistemaAudio != null)
            SistemaAudio.PlayOneShot(Audio);
    }

    public void Stop()
    {
        if (m_CorotineAnim != null)
        {
            StopCoroutine(m_CorotineAnim);
            m_CorotineAnim = null;
        }

        IsPlaying = false;
        IsDone = true;
        m_IndexSprite = 0;

        if (m_Image != null)
            m_Image.enabled = false;

        if (Audio != null && SistemaAudio != null)
            SistemaAudio.Stop();
    }

    public void PlayUIAnim()
    {
        if (IsPlaying)
            Stop();
        else
            Play();
    }

    IEnumerator Func_PlayAnimUI()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_Speed);

            if (m_IndexSprite >= m_SpriteArray.Length)
            {
                if (Loop)
                {
                    m_IndexSprite = 0;
                }
                else
                {
                    m_IndexSprite = m_SpriteArray.Length - 1;
                    m_Image.sprite = m_SpriteArray[m_IndexSprite];
                    IsPlaying = false;
                    IsDone = true;
                    m_CorotineAnim = null;
                    yield break;
                }
            }

            m_Image.sprite = m_SpriteArray[m_IndexSprite];
            m_IndexSprite += 1;
        }
    }
}