using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
  public NetworkPlayer player;

  void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
    NetworkPlayer Splayer = player;
    if (stream.isWriting) {
      Splayer = player;
      stream.Serialize(ref Splayer);
    } else {
      stream.Serialize(ref Splayer);
      player = Splayer;
    }
  }
}
