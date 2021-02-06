using Mirror;
using Mirror.Discovery;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public struct ServerRxRequest : NetworkMessage { }
public struct ServerRxResponse : NetworkMessage
{
    public string version;
    public IPEndPoint EndPoint { get; set; }
    public Uri uri;
    public long serverId;
    /// <summary>
    /// Player name
    /// </summary>
    public string name;
}
public enum NetworkDiscoverStatus
{
    Finding=2,
    Connected=1,
    Disabled=0,
    Closed=0
}
[DisallowMultipleComponent]
[AddComponentMenu("Network/NetworkDiscovery")]
public class NetworkRxDiscover : NetworkDiscoveryBase<ServerRxRequest, ServerRxResponse>
{
    public static NetworkRxDiscover S;
    readonly Dictionary<long, ServerRxResponse> discoveredServers = new Dictionary<long, ServerRxResponse>();

    public long ServerId { get; private set; }
    [Tooltip("Transport to be advertised during discovery")]
    public Transport transport;

    [Header("UI")]
    public Text text;
    public GameObject networkObjectPrefab;
    public Transform networkPanel;

    [Header("NetworkStatus")]
    public Animator animator;
    public Sprite bad;
    public Sprite middle;
    public Sprite good;
    public Sprite none;
    public UIButton abledConnect;
    public UIButton disabledConnect;
    [Space(10)]
    [SerializeField] bool isDiscovering = false;
    public override void Start()
    {
        ServerId = RandomLong();
        if (transport == null)
            transport = Transport.activeTransport;
        S = this;
        base.Start();
        
    }
    public void OnNetworkButtonClicked(bool isFinded=false)
    {
        print(NetworkControl.isNetworkAvailable);
        if (isFinded) animator.SetInteger("NetworkDiscoveryStatusEnum", (int)NetworkDiscoverStatus.Connected);
        else if (!NetworkControl.isNetworkAvailable)
            animator.SetInteger("NetworkDiscoveryStatusEnum", (int)NetworkDiscoverStatus.Disabled);
        else
        {
            print(isDiscovering);
            if (isDiscovering)
            {
                StopDiscovery();
                isDiscovering = false;
                animator.SetInteger("NetworkDiscoveryStatusEnum", (int)NetworkDiscoverStatus.Closed);
            }
            else
            {
                print(1);
                StartDiscovery();
                isDiscovering = true;
                animator.SetInteger("NetworkDiscoveryStatusEnum", (int)NetworkDiscoverStatus.Finding);
            }
        }
    }
    /// <summary>
    /// Use Android plugin to get wifi SSID
    /// </summary>
    /// <returns>Wifi SSID</returns>
    public static string GetWifiSSID()
    {
#if UNITY_ANDROID
        AndroidJavaClass main = new AndroidJavaClass("com.syz.unitygamePlugin.Main");
        return main.Call<string>("GetNowWifiSSID");
#else
        return "";
#endif
    }
    public void NetworkPanel(bool isOpen)
    {
        if (isOpen)
        {
            if(NetworkControl.isNetworkAvailable)
            text.text = $"SSID:{GetWifiSSID()}";
            networkPanel.gameObject.SetActive(true);
        }
        else networkPanel.gameObject.SetActive(false);
    }
    /// <summary>
    /// Process the request from a client
    /// </summary>
    /// <remarks>
    /// Override if you wish to provide more information to the clients
    /// such as the name of the host player
    /// </remarks>
    /// <param name="request">Request comming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>
    /// <returns>The message to be sent back to the client or null</returns>
    protected override ServerRxResponse ProcessRequest(ServerRxRequest request, IPEndPoint endpoint)
    {
        try
        {
            return new ServerRxResponse
            {
                version = Application.version,
                serverId = ServerId,
                uri = transport.ServerUri(),
                name = Guid.NewGuid().ToString("D")
            };
        }
        catch (NotImplementedException)
        {
            Debug.LogError($"Transport {transport} does not support network discovery");
            throw;
        }
    }
    protected override ServerRxRequest GetRequest() => new ServerRxRequest();

    /// <summary>
    /// Process the answer from a server
    /// </summary>
    /// <remarks>
    /// A client receives a reply from a server, this method processes the
    /// reply and raises an event
    /// </remarks>
    /// <param name="response">Response that came from the server</param>
    /// <param name="endpoint">Address of the server that replied</param>
    protected override void ProcessResponse(ServerRxResponse response, IPEndPoint endpoint)
    {
        response.EndPoint = endpoint;
        UriBuilder realUri = new UriBuilder(response.uri)
        {
            Host = response.EndPoint.Address.ToString()
        };
        response.uri = realUri.Uri;
        // Add ServerResponse to dictionary
        discoveredServers.Add(response.serverId, response);
        // Spawn the column of NetworkObject
        GameObject networkObject = Instantiate(networkObjectPrefab, 
            new Vector3(0, 40 - (60 * discoveredServers.Count)), new Quaternion(), networkPanel);
        NetworkObject networkObjectScript = networkObject.GetComponent<NetworkObject>();
        networkObjectScript.isVersionCorrectly = response.version == Application.version;
        networkObjectScript.ip = response.uri.AbsoluteUri;
        networkObjectScript.objectName = response.name;
        networkObjectScript.UpdateUI();
    }
}