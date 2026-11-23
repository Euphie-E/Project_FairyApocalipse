
using System.Collections;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rockDebris;
    [SerializeField] private ShadowEffect shadowEffect;

    [Header("Drop Settings")]
    [SerializeField] private float delayBeforeDrop = 2f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float size = 2f;

    private Rigidbody rockDebrisRB;
    private Vector3 initialPosition;
    private Coroutine dropCoroutine;

    private void Start()
    {
        rockDebrisRB = rockDebris.GetComponent<Rigidbody>();

        initialPosition = rockDebris.localPosition;

        rockDebris.gameObject.SetActive(false);
        shadowEffect.HideShadow();
    }

    public void Drop(float startHeight, bool randomize)
    {
        if (dropCoroutine != null)
            StopCoroutine(dropCoroutine);

        dropCoroutine = StartCoroutine(
            DropRoutine(startHeight, randomize)
        );
    }

    private IEnumerator DropRoutine(float startHeight, bool randomize)
    {
        // RESET
        rockDebris.gameObject.SetActive(false);

        rockDebris.localPosition = initialPosition;

        rockDebrisRB.linearVelocity = Vector3.zero;
        rockDebrisRB.angularVelocity = Vector3.zero;

        float dropSize = randomize
            ? Random.Range(1f, 4f)
            : size;

        rockDebris.localScale = Vector3.one * dropSize;

        // 1. MOSTRA A SOMBRA
        shadowEffect.ShowWarning(rockDebris, startHeight);

        // 2. ESPERA X SEGUNDOS
        yield return new WaitForSeconds(delayBeforeDrop);

        // 3. ATIVA A PEDRA
        rockDebris.gameObject.SetActive(true);

        // 4. INICIA A QUEDA
        shadowEffect.StartDrop();

        rockDebrisRB.useGravity = false;

        float dropSpeed = randomize
            ? Random.Range(3f, 6f)
            : speed;

        rockDebrisRB.linearVelocity = Vector3.down * dropSpeed;

        dropCoroutine = null;
    }
}
