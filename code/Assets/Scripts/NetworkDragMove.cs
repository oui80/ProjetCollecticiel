using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkDragMove : MonoBehaviourPun
{
    private Camera cam;
    private bool dragging = false;

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        // Si ce n'est pas moi qui possède l'objet, je demande l'ownership
        if (!photonView.IsMine)
        {
            Debug.Log("[" + PhotonNetwork.NickName + "] demande ownership de " + gameObject.name);
            photonView.RequestOwnership();
        }

        dragging = true;
    }

    void OnMouseUp()
    {
        dragging = false;
    }

    void Update()
    {
        // Très important : seul le propriétaire bouge l'objet
        if (!photonView.IsMine || !dragging)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        // Plan horizontal (comme une table) à y = 0
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = hitPoint;
        }
    }
}
