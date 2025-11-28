using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkTransformSync : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private float lerpSpeed = 10f;   // <- deviens private, mais toujours réglable dans l’inspecteur

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    void Start()
    {
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * lerpSpeed);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // je suis owner : j'envoie
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else // je reçois
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
