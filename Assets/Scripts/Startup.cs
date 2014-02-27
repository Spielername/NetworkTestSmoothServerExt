using UnityEngine;
using System.Collections;

public class Startup : MonoBehaviour
{
  private const string typeName = "MAHN42";
  private string levelName = "01";
  private bool isLoadingHosts = false;
  private bool isClientStart = false;
  private bool isLocal = false;
  private HostData[] hostList;
  public GameObject playerPrefab = null;

  void OnGUI ()
  {
    if (!Network.isClient && !Network.isServer && !isClientStart && !isLocal) {
      //levelName = GUI.TextField (new Rect (100, 25, 100, 25), levelName);   
      if (GUI.Button (new Rect (100, 100, 250, 100), "Start Server"))
        StartServer ();
      
      if (GUI.Button (new Rect (100, 210, 250, 100), "Start Client"))
        isClientStart = true;

      if (GUI.Button (new Rect (100, 320, 250, 100), "Start Local")) {
        isLocal = true;
        SpawnPlayer ( new NetworkPlayer());
      }
    } else {
      if (Network.isServer) {
        GUI.Label (new Rect (100, 100, 250, 100), "Server");
      } else if (isClientStart) {
        GUI.Label (new Rect (100, 100, 250, 100), "Client");
        if (!Network.isClient) {
          if (!isLoadingHosts) {
            isLoadingHosts = true;
            MasterServer.RequestHostList (typeName);
          } else {
            if (hostList == null) {
              LoadHostList ();
            } else {
              if (hostList != null) {
                if (hostList.Length == 0) {
                  GUI.Label (new Rect (100, 200, 250, 100), "searching server...");
                  LoadHostList ();
                } else {
                  if (GUI.Button (new Rect (100, 100, 250, 100), "request again")) {
                    hostList = null;
                    isLoadingHosts = false;
                    MasterServer.ClearHostList ();
                  } else {
                    if (hostList != null) {
                      for (int i = 0; i < hostList.Length; i++) {
                        if (GUI.Button (new Rect (400, 100 + (110 * i), 300, 100), hostList [i].gameName + "/" + hostList [i].gameType))
                          JoinServer (hostList [i]);
                      }
                    }
                  }
                }
              } else {
                if (GUI.Button (new Rect (100, 100, 250, 100), "request again")) {
                  hostList = null;
                  isLoadingHosts = false;
                  MasterServer.ClearHostList ();
                }
              }
            }
          }
        }
      }
    }
  }
  
  private void StartServer ()
  {
    Network.InitializeServer (5, 25000, !Network.HavePublicAddress ());
    MasterServer.RegisterHost (typeName, levelName);
  }
  
  private void LoadHostList ()
  {
    isLoadingHosts = false;
    hostList = MasterServer.PollHostList ();
  }
  
  private void JoinServer (HostData hostData)
  {
    Network.Connect (hostData);
    hostList = null;
  }

  public class PlayerGameObject {
    public NetworkPlayer player;
    public GameObject gameobject;

    public PlayerGameObject(NetworkPlayer aPlayer, GameObject aObject) {
      player = aPlayer;
      gameobject = aObject;
    }
  }

  public ArrayList playerGameObjects = new ArrayList();

  public void SpawnPlayer (NetworkPlayer player)
  {
    GameObject lPlayer;
    if (isLocal) {
      lPlayer = Instantiate (playerPrefab, new Vector3 (0, 2.0f, 0), Quaternion.identity) as GameObject;
    } else {
      lPlayer = Network.Instantiate (playerPrefab, new Vector3 (0, 8.0f, 0), Quaternion.identity, 0) as GameObject;
    }
    lPlayer.GetComponent<PlayerController>().player = player;
    PlayerGameObject lP = new PlayerGameObject(player, lPlayer);
    playerGameObjects.Add(lP);
  }

  public GameObject getPlayerGameObject(string aGuid) {
    foreach(PlayerGameObject lP in playerGameObjects) {
      if (isLocal || lP.player.guid.Equals(aGuid)) {
        return lP.gameobject;
      }
    }
    return null;
  }

  // called on client side only
  void OnConnectedToServer ()
  {
    Debug.Log ("CTS Player " + Network.player.guid + " connected from " + Network.player.ipAddress + ":" + Network.player.port);
    //SpawnPlayer ();
  }
  
  void OnPlayerDisconnected (NetworkPlayer player)
  {
    Debug.Log ("Clean up after player " + player);
    GameObject lObj = getPlayerGameObject(player.guid);
    if (lObj != null) {
      DestroyObject(lObj);
    }
    Network.RemoveRPCs (player);
    Network.DestroyPlayerObjects (player);
  }

  void OnPlayerConnected (NetworkPlayer player)
  {
    Debug.Log ("PC Player " + player.guid + " connected from " + player.ipAddress + ":" + player.port);
    SpawnPlayer (player);
  }
}
