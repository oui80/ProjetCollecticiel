using Photon.Pun;
using UnityEngine;
using MixedReality.Toolkit.UX; // StatefulInteractable

public class MasterClickToColor : MonoBehaviourPun
{
    private Renderer[] renderers;
    [SerializeField] private Color clickColor = Color.red;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    // Brancher cette méthode dans StatefulInteractable -> OnClicked
    public void OnClicked()
    {
        if (!PhotonNetwork.IsMasterClient) return; // seuls le serveur/master

        photonView.RPC(nameof(ApplyColor_All), RpcTarget.AllBuffered, clickColor.r, clickColor.g, clickColor.b, 1f);
    }

    [PunRPC]
    private void ApplyColor_All(float r, float g, float b, float a)
    {
        var c = new Color(r, g, b, a);
        foreach (var rend in renderers)
        {
            if (rend == null) continue;
            var mats = rend.materials;
            for (int i = 0; i < mats.Length; i++)
                mats[i].color = c;
            rend.materials = mats;
        }
    }
}

