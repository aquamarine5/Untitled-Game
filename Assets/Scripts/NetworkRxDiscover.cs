using Mirror;
using Mirror.Discovery;
using System;
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
}

[DisallowMultipleComponent]
[AddComponentMenu("Network/NetworkDiscovery")]
public class NetworkRxDiscover : NetworkDiscoveryBase<ServerRxRequest, ServerRxResponse>
{

    public long ServerId { get; private set; }
    [Tooltip("Transport to be advertised during discovery")]
    public Transport transport;
    [Header("UI")]
    public Text text;

    public override void Start()
    {
        ServerId = RandomLong();
        if (transport == null)
            transport = Transport.activeTransport;

        base.Start();
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
                uri = transport.ServerUri()
            };
        }
        catch (NotImplementedException)
        {
            Debug.LogError($"Transport {transport} does not support network discovery");
            throw;
        }
    }

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
        if (response.version != Application.version)
        {

        }
    }
}