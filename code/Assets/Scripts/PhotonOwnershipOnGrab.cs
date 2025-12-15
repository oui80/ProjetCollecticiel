using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PhotonOwnershipOnGrab : MonoBehaviourPun
{
    private Renderer cubeRenderer;
    private Color originalColor;

    private void Awake()
    {
        cubeRenderer = GetComponentInChildren<Renderer>();
        if (cubeRenderer != null)
        {
            originalColor = cubeRenderer.material.color;
        }
        else
        {
            originalColor = Color.white;
            Debug.LogWarning($"{nameof(PhotonOwnershipOnGrab)}: No Renderer found in children on '{name}'.");
        }

        // Prevent MRTK3 ScaleLogic from running when only one interactor is selecting,
        // which can cause ArgumentOutOfRangeException in ScaleLogic.Setup().
        DisableScaleOnMrtkObjectManipulator();
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

    // Appel� par ton ObjectManipulator
    public void OnGrabStarted()
    {
        if (!photonView.IsMine)
        {
            photonView.RequestOwnership();
            Debug.Log("Requested ownership of cube");
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // On change la couleur
            photonView.RPC("ChangeCubeColor", RpcTarget.AllBuffered);
            GameObject obj = this.gameObject;

            Component manipulator =
                    obj.GetComponent("Microsoft.MixedReality.Toolkit.UI.ObjectManipulator") ??
                    obj.GetComponent("MixedReality.Toolkit.SpatialManipulation.ObjectManipulator");

            if (manipulator is Behaviour behaviour)
            {
                StartCoroutine(DisableNextFrame(behaviour));
            }
        }
    }

    public void OnGrabEnded()
    {
        //photonView.RPC("RestoreOriginalColor", RpcTarget.AllBuffered);
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
        if (cubeRenderer != null)
        {
            cubeRenderer.material.color = originalColor;
        }
    }

}