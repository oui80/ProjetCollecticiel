using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RequestOwnershipOnClick : MonoBehaviourPun
{
    void OnMouseDown()
    {
        Debug.Log("OnMouseDown sur " + gameObject.name + " - IsMine=" + photonView.IsMine);

        if (!photonView.IsMine)
        {
            Debug.Log("Je demande l'ownership");
            photonView.RequestOwnership();
        }
    }
}
