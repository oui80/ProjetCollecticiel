using UnityEngine;
using Photon.Pun;

public class PhotonOwnershipOnGrab : MonoBehaviour
{
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // This method will be called from an event on ObjectManipulator
    public void OnGrabStarted()
    {
        if (photonView != null && !photonView.IsMine)
        {
            // Ask Photon to give this client ownership of the cube
            photonView.RequestOwnership();
            Debug.Log("Requested ownership of cube");
        }
    }
}