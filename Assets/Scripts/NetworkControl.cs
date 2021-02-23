using Mirror;
using Mirror.Experimental;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using NetworkTransformBase = Mirror.NetworkTransformBase;

public class NetworkControl : NetworkManager
{
    /// <summary>
    /// If the network environment is WIFI return true.<br/>
    /// <seealso cref="Application.internetReachability"/>
    /// </summary>
    public static bool IsNetworkAvailable => Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
    public static NetworkControl S { get; private set; }
    public NetworkRxDiscover discover;
    [Header("Player Require Argument")]
    public GameObject disconnectedPlayer;
    public Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera;
    public PlayerMove playerMove;

    public override void Awake()
    {
        S = this;
        base.Awake();
    }
    public void RxStartHost()
    {
        if (IsNetworkAvailable)
        {
            discover.AdvertiseServer();
            StartHost();
        }
        else
        {
            Debug.LogError("FAILED");
            NetworkRxDiscover.S.text.text = 
                $"<color=red>{(Application.internetReachability == NetworkReachability.NotReachable ? "您的设备未连接到网络" : "您的设备使用移动数据，请切换到WiFi")}</color>";
        }
    }
    public override void OnStartClient()
    {
        if (IsNetworkAvailable)
            base.OnStartClient();
        else
        {
            Debug.LogError("FAILED");
        }
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        /// when <see cref="NetworkServer.connections.Count"/> ==0 meaning <seealso cref="NetworkManager.StartHost"/>
        NetworkServer.AddPlayerForConnection(conn, NetworkServer.connections.Count == 0 
            ? disconnectedPlayer
            : Instantiate(playerPrefab, GetStartPosition().position , new Quaternion()));
    }
}

public enum NetworkPingStatus
{
    /// <summary>
    /// <see cref="NetworkRxDiscover.bad"/>
    /// </summary>
    Bad = 0,
    /// <summary>
    /// <see cref="NetworkRxDiscover.middle"/>
    /// </summary>
    Middle = 1,
    /// <summary>
    /// <see cref="NetworkRxDiscover.good"/>
    /// </summary>
    Good = 2,
    /// <summary>
    /// <see cref="NetworkRxDiscover.none"/>
    /// </summary>
    None = 100
}

public static class NetworkPlugin
{
    public static void SetNetworkComponentActive(this NetworkIdentity networkIdentity, bool isActive)
    {
        GameObject gameObject = networkIdentity.gameObject;
        gameObject.GetComponent<NetworkAnimator>().SetEnabled(isActive);
        gameObject.GetComponent<NetworkRigidbody>().SetEnabled(isActive);
        gameObject.GetComponent<NetworkRigidbody2D>().SetEnabled(isActive);
        gameObject.GetComponent<NetworkTransformBase>().SetEnabled(isActive);
    }
    static void SetEnabled(this MonoBehaviour monoBehaviour, bool isEnabled) 
    {
        if (monoBehaviour == null) return;
        monoBehaviour.enabled = isEnabled; 
    }
}