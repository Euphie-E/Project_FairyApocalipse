using Unity.Cinemachine;
using UnityEngine;

public class CameraCollisionManager : MonoBehaviour
{
    [SerializeField] private CinemachineDeoccluder deoccluder;
    [SerializeField] private TimeTravel timeTravel;

    void Start()
    {
        if (deoccluder == null) deoccluder = GetComponent<CinemachineDeoccluder>();
        if (timeTravel == null) timeTravel = transform.Find("PlayerCharacter").GetComponent<TimeTravel>();
    }

    void Update()
    {
        // LayerMask(65) Passado
        // LayerMask(129) Futuro

        if (timeTravel.isTravelling && deoccluder.CollideAgainst.value != 129) // futuro
        {
            deoccluder.CollideAgainst.value = 129;
        }

        else if (!timeTravel.isTravelling && deoccluder.CollideAgainst.value != 65) // Passado
        {
            deoccluder.CollideAgainst.value = 65;
        }
    }
}
