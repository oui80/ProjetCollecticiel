using UnityEngine;
using Microsoft.MixedReality.OpenXR;
using UnityEngine.XR.ARSubsystems;

public class QRCodeReader : MonoBehaviour
{
    public ARMarkerManager markerManager;
    public GameObject table;
    public bool init = false;

    private void Start()
    {
        init = false;
        if (markerManager == null)
        {
            Debug.LogError("ARMarkerManager is not assigned.");
            return;
        }

        // Subscribe to the markersChanged event
        markerManager.markersChanged += OnMarkersChanged;
    }

    /// <summary>
    /// Handles the markersChanged event and processes added, updated, and removed markers.
    /// </summary>
    /// <param name="args">Event arguments containing information about added, updated, and removed markers.</param>
    private void OnMarkersChanged(ARMarkersChangedEventArgs args)
    {
        foreach (var addedMarker in args.added)
        {
            HandleAddedMarker(addedMarker);
        }

        foreach (var updatedMarker in args.updated)
        {
            HandleUpdatedMarker(updatedMarker);
        }

        foreach (var removedMarkerId in args.removed)
        {
            HandleRemovedMarker(removedMarkerId);
        }
    }

    /// <summary>
    /// Handles logic for newly added markers.
    /// </summary>
    /// <param name="addedMarker">The newly added ARMarker.</param>
    private void HandleAddedMarker(ARMarker addedMarker)
    {
        Debug.Log($"QR Code Detected! Marker ID: {addedMarker.trackableId}");

        // Get the position of the QR code
        table.transform.position = addedMarker.transform.position;
    }

    /// <summary>
    /// Handles logic for updated markers.
    /// </summary>
    /// <param name="updatedMarker">The updated ARMarker.</param>
    private void HandleUpdatedMarker(ARMarker updatedMarker)
    {
        Debug.Log($"QR Code updated! Marker ID: {updatedMarker}");

        //// Get the position of the QR code
        table.transform.position = updatedMarker.transform.position;

        if (updatedMarker.trackingState == TrackingState.Tracking)
        {
            init = true;
        }

    }

    /// <summary>
    /// Handles logic for removed markers.
    /// </summary>
    /// <param name="removedMarkerId">The ID of the removed marker.</param>
    private void HandleRemovedMarker(ARMarker removedMarkerId)
    {
        //Debug.Log($"QR Code Removed! Marker ID: {removedMarkerId}");
    }

}