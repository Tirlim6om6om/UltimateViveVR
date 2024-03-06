using Mirror.Discovery;
using UnityEngine;
using UnityEngine.UI;

namespace BCS.CORE.VR.Network.Example
{
    public class ServerElementUI : MonoBehaviour
    {
        [SerializeField] private Text textIp;
        private ServerResponse _info;
        
        public void SetIP(ServerResponse server)
        {
            _info = server;
            textIp.text = _info.uri.AbsoluteUri;
        }

        public ServerResponse GetIP()
        {
            return _info;
        }
        
        public void Connect()
        {
            ServerList.instance.Connect(_info);
        }
    }
}
