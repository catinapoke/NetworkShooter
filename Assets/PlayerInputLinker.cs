using System;
using Unity.Netcode;

public class PlayerInputLinker : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            GetComponent<MoreMountains.CorgiEngine.Character>().PlayerID = String.Empty;
        }
    }
}
