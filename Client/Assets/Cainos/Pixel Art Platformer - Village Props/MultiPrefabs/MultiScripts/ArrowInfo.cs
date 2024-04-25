using Unity.Netcode;
using UnityEngine;

public class ArrowInfo : MonoBehaviour
{
    public ulong OwnerClientId { get; private set; }

    public void SetOwnerClientId(ulong clientId)
    {
        OwnerClientId = clientId;
    }
}
