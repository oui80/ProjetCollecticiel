using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class DragWithMouseNetwork : MonoBehaviourPun
{
    private Camera cam;
    private bool dragging = false;
    private float distanceToPlane = 2f;

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        // Je prends l'ownership si ce n'est pas déjà le cas
        if (!photonView.IsMine)
        {
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
        // Très important : seul le propriétaire peut déplacer
        if (!photonView.IsMine || !dragging)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        // On projette sur un plan horizontal devant la caméra
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = hitPoint;
        }
    }
}
