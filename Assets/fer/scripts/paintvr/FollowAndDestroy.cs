// FollowAndDestroy.cs
using UnityEngine;
using Unity.Netcode;

public class FollowAndDestroy : MonoBehaviour
{
    public NetworkObject masterObject;

    void Update()
    {
        if (masterObject == null || !masterObject.IsSpawned)
        {
            Destroy(gameObject);
        }
    }
}