using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PhotonOwnershipOnGrab : MonoBehaviourPun
{
    private Renderer[] renderers;
    private Color[][] originalColors;


    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);

        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogWarning($"{nameof(PhotonOwnershipOnGrab)}: No Renderer found in children on '{name}'.");
        }
        else
        {
            originalColors = new Color[renderers.Length][];
            for (int r = 0; r < renderers.Length; r++)
            {
                var mats = renderers[r].materials;
                originalColors[r] = new Color[mats.Length];
                for (int m = 0; m < mats.Length; m++)
                    originalColors[r][m] = mats[m].color;
            }
        }

    }

    private void DisableScaleOnMrtkObjectManipulator()
    {
        Component manipulator =
                gameObject.GetComponent("MixedReality.Toolkit.SpatialManipulation.ObjectManipulator") ??
                gameObject.GetComponent("Microsoft.MixedReality.Toolkit.UI.ObjectManipulator");

        if (manipulator == null)
        {
            return;
        }

        // Use reflection to avoid hard dependency on a specific MRTK version/API surface.
        TrySetEnumFlagsProperty(manipulator, "AllowedManipulations", new[] { "Move", "Rotate" });
        TrySetEnumFlagsProperty(manipulator, "AllowedTransformations", new[] { "Move", "Rotate" });
        TrySetEnumFlagsProperty(manipulator, "AllowedInteractionTypes", new[] { "Move", "Rotate" });
    }

    private static void TrySetEnumFlagsProperty(Component component, string propertyName, string[] allowedFlagNames)
    {
        var prop = component.GetType().GetProperty(propertyName);
        if (prop == null || !prop.CanWrite || !prop.PropertyType.IsEnum)
        {
            return;
        }

        try
        {
            long combined = 0;
            foreach (var flagName in allowedFlagNames)
            {
                combined |= Convert.ToInt64(Enum.Parse(prop.PropertyType, flagName, ignoreCase: true));
            }

            prop.SetValue(component, Enum.ToObject(prop.PropertyType, combined));
        }
        catch
        {
            // Ignore if the MRTK version doesn't expose these enum values/properties.
        }
    }

    private IEnumerator DisableNextFrame(Behaviour behaviour)
    {
        yield return null;
        if (behaviour != null)
        {
            behaviour.enabled = false;
        }
    }

    // Appel? par ton ObjectManipulator
    public void OnGrabStarted()
    {
        // Master : change couleur + désactive sa manipulation
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ChangeCubeColor", RpcTarget.AllBuffered);

            Component manipulator =
                gameObject.GetComponent("MixedReality.Toolkit.SpatialManipulation.ObjectManipulator") ??
                gameObject.GetComponent("Microsoft.MixedReality.Toolkit.UI.ObjectManipulator");

            if (manipulator is Behaviour behaviour)
                StartCoroutine(DisableNextFrame(behaviour));

            return; // IMPORTANT : le Master s'arrête ici
        }

        // Client : demande ownership pour pouvoir bouger
        if (!photonView.IsMine)
        {
            photonView.RequestOwnership();
            Debug.Log("Requested ownership of cube");
        }
    }


    public void OnGrabEnded()
    {
        //photonView.RPC("RestoreOriginalColor", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void ChangeCubeColor()
    {
        if (renderers == null) return;

        for (int r = 0; r < renderers.Length; r++)
        {
            var mats = renderers[r].materials;
            for (int m = 0; m < mats.Length; m++)
            {
                var c = Color.red; c.a = 1f;
                mats[m].color = c;
            }
            renderers[r].materials = mats;
        }
    }


    [PunRPC]
    void RestoreOriginalColor()
    {
        if (renderers == null || originalColors == null) return;

        for (int r = 0; r < renderers.Length; r++)
        {
            var mats = renderers[r].materials;
            for (int m = 0; m < mats.Length && m < originalColors[r].Length; m++)
                mats[m].color = originalColors[r][m];

            renderers[r].materials = mats;
        }
    }


}