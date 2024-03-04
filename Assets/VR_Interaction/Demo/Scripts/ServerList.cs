using System.Collections.Generic;
using Mirror;
using Mirror.Discovery;
using UnityEngine;
using UnityEngine.Events;

namespace BCS.CORE.VR.Network.Example
{
    public class ServerList : MonoBehaviour
    {
        public static ServerList instance;
        [SerializeField] private GameObject prefab;
        [SerializeField] private NetworkDiscovery networkDiscovery;
        [SerializeField] private UnityEvent onConnected;
        private List<ServerElementUI> _elements;

        private void Start()
        {
            if (instance)
            {
                Destroy(instance);
            }
            else
            {
                instance = this;
            }
            networkDiscovery.OnServerFound.AddListener(UpdateList);
        }

        private void UpdateList(ServerResponse server)
        {
            ServerElementUI elementServer = Instantiate(prefab, transform).GetComponent<ServerElementUI>();
            elementServer.SetIP(server);
        }

        public void Connect(ServerResponse server)
        {
            NetworkManager.singleton.StartClient(server.uri);
            onConnected.Invoke();
            networkDiscovery.StopDiscovery();
        }

        public void Host()
        {
            NetworkManager.singleton.StartHost();
            networkDiscovery.AdvertiseServer();
            onConnected.Invoke();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Host();
            }
        }
#endif
    }
}
