using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RadarController : MonoBehaviour {

    public GlobalSettings globalSettings;

	public Transform radarUIElement;

	private IList<Transform> TrackedObjects;

	private IList<Transform> TrackingIcons;

	public Transform TrackingCentre;

	public Vector2 forwardVector;

	public float maxRadarRange = 160.0f;

	public float radarUIRadius = 4.0f;

	private float radarScalingFactor = 0.0f;

	// Use this for initialization
	void Start () 
    {
		TrackedObjects = new List<Transform> ();
		TrackingIcons = new List<Transform> ();
		radarScalingFactor = radarUIRadius / maxRadarRange;
        if (!globalSettings.EnableRadar)
        {
            this.GetComponent<Image>().enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (globalSettings.EnableRadar)
        {
            IList<int> indicesPendingRemoval = new List<int>();
            for (int i = 0; i < TrackedObjects.Count; i++)
            {
                TrackingIcons[i].rotation = radarUIElement.rotation;
                if (TrackedObjects[i] != null)
                {
                    //determine the ghost's relative position to the player
                    Vector3 targetOffset = TrackedObjects[i].position - TrackingCentre.position;
                    //translate that into a vector for displacement on the radar
                    //this gets the displacement vector from the centre of the radar (may need to adjust depending on where the centre of the transform falls)
                    Vector2 radarVector = new Vector2(targetOffset.x, targetOffset.z) * radarScalingFactor;
                    //cap the vector to make sure the ghost does not go off the radar
                    if (radarVector.magnitude > radarUIRadius / 2)
                    {
                        radarVector = radarVector.normalized * radarUIRadius / 2;
                    }
                    //calculate any difference between the forward vector we have used for these calculations (1,0), and our actual forward vector
                    float rotationAngle = Vector2.Angle(new Vector2(1.0f, 0.0f), forwardVector);
                    //Debug.Log (string.Format("RadarVector x: {0}, y: {1}, rotationAngle: {2}", radarVector.x, radarVector.y, rotationAngle));
                    //move the icon
                    TrackingIcons[i].position = radarUIElement.position + new Vector3(-0.01f, radarVector.x, radarVector.y);
                    TrackingIcons[i].RotateAround(radarUIElement.position, new Vector3(forwardVector.x, 0.0f, forwardVector.y), rotationAngle);
                }
                else
                {
                    indicesPendingRemoval.Add(i);
                }
            }
            //cleanup any tracked objects that no longer exist (count down so we remove higher indexed objects first and don't muck up indexes of other objects to be removed)
            for (int index = indicesPendingRemoval.Count - 1; index >= 0; index--)
            {
                TrackedObjects.RemoveAt(indicesPendingRemoval[index]);
                GameObject.Destroy(TrackingIcons[indicesPendingRemoval[index]].gameObject);
                TrackingIcons.RemoveAt(indicesPendingRemoval[index]);
            }
        }
	}

	public void CleanUp() {
		//delete all the ghosts
		for (int i = 0; i < TrackingIcons.Count; i++) {
			GameObject.Destroy(TrackingIcons[i].gameObject);
		}
		TrackingIcons.Clear ();
		TrackedObjects.Clear ();
	}

	public void RegisterObjectForTracking(Transform toRegister, Transform icon) {
        if (globalSettings.EnableRadar)
        {
            if (toRegister != null && icon != null)
            {
                TrackedObjects.Add(toRegister);
                TrackingIcons.Add(icon);
            }
            else
            {
                Debug.Log("RadarController: Attempting to register null object or icon.");
            }
        }
	}

	public void RequestHideTrackingIcon(Transform associatedObject) {
        if (globalSettings.EnableRadar)
        {
            int index = TrackedObjects.IndexOf(associatedObject);
            TrackingIcons[index].GetComponent<Renderer>().enabled = false;
        }
	}

	public void RequestShowTrackingIcon(Transform associatedObject) {
        if (globalSettings.EnableRadar)
        {
            int index = TrackedObjects.IndexOf(associatedObject);
            TrackingIcons[index].GetComponent<Renderer>().enabled = true;
        }
	}
}