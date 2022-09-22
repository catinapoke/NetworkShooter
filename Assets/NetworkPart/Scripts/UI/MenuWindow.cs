using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameUI
{
    public class MenuWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text _port;
        [SerializeField] private TMP_Text _ip;

        private NetworkManager _manager;
        private UnityTransport _transport;

        private void Start()
        {
            _manager = NetworkManager.Singleton;
            _transport = _manager.NetworkConfig.NetworkTransport as UnityTransport;

            if (_manager == null) Debug.LogError("[MenuWindow] Manager is null!");
            if (_transport == null) Debug.LogError("[MenuWindow] Transport is null!");
        }

        public void StartServer()
        {
            int port;
            // TextMeshPro adds empty symbol with id 8203 that gives a FormatException
            if (!int.TryParse(_port.text.TrimEnd((char) 8203), out port))
            {
                Debug.LogError("Can't parse port");
                return;
            }

            _transport.SetConnectionData(
                "127.0.0.1", // The IP address is a string
                (ushort) port, // The port number is an unsigned short
                "0.0.0.0" // The server listen address is a string.
            );

            if (_manager.StartHost())
            {
                // Start controlling callbacks of client load
                // Load lobby scene
                NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("Can't host server!");
            }
        }

        public void JoinServer()
        {
            UnityTransport transport = _manager.NetworkConfig.NetworkTransport as UnityTransport;

            if (transport == null)
            {
                Debug.LogError("Wrong transport type - can't set connection data!");
                return;
            }

            string[] ip_port = _ip.text.TrimEnd((char) 8203).Split(":");

            if (ip_port.Length != 2)
            {
                Debug.LogError("Can't parse ip - wrong length!");
                return;
            }

            transport.SetConnectionData(ip_port[0], ushort.Parse(ip_port[1]));

            if (_manager.StartClient())
            {
                // Load lobby scene
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Lobby");
            }
            else
            {
                Debug.LogError("Can't join server!");
            }
        }

        public void Exit()
        {
            Debug.Log("Clicked exit button");
            Application.Quit();
        }
    }
}