using Photon.Pun;
using UnityEngine;

public class PlayerMove : MonoBehaviourPun
{
    void Update()
    {
        if (!photonView.IsMine) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(h, 0, v) * Time.deltaTime * 5f);
    }
}
