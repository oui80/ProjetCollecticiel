using UnityEngine;
using Photon.Pun;

public class PhotonOwnershipOnGrab : MonoBehaviourPun
{
    private Renderer cubeRenderer;
    private Color originalColor;

    private void Awake()
    {
        cubeRenderer = GetComponent<Renderer>();
        originalColor = cubeRenderer.material.color;
    }

    // Appelé par ton ObjectManipulator
    public void OnGrabStarted()
    {
        if (!photonView.IsMine)
        {
            photonView.RequestOwnership();
            Debug.Log("Requested ownership of cube");
        }

        if (PhotonNetwork.IsMasterClient) {
            // On change la couleur
            photonView.RPC("ChangeCubeColor", RpcTarget.AllBuffered);
        }
    }

    public void OnGrabEnded()
    {
        photonView.RPC("RestoreOriginalColor", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void ChangeCubeColor()
    {
        if (cubeRenderer != null)
        {
            cubeRenderer.material.color = Color.red;
        }
    }

    [PunRPC]
    void RestoreOriginalColor()
    {
        cubeRenderer.material.color = Color.blue;
    }

}