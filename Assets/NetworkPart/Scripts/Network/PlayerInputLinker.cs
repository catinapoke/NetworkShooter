using System;
using MoreMountains.CorgiEngine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameNetwork
{
    public class PlayerInputLinker : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log($"OnNetworkSpawn {this.GetType().Name} Scene: {SceneManager.GetActiveScene().name}");
                
            Character character = GetComponent<Character>();
            if (!IsOwner)
            {
                character.SetPlayerID("None");
                return;
            }

            if (!IsServer)
            {
                // In case of long load
                if (InputManager.TryGetInstance() == null)
                {
                    NetworkManager.SceneManager.OnLoadComplete += OnSceneLoaded;
                }
            }
                
        }

        private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if(clientId != OwnerClientId) return;
            NetworkManager.SceneManager.OnLoadComplete -= OnSceneLoaded;
            
            Character character = GetComponent<Character>();
            character.SetInputManager(InputManager.TryGetInstance());
        }
        
    }
}