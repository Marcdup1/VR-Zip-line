using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WandController : MonoBehaviour {
	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	private Valve.VR.EVRButtonId touchpadButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

	private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
	private SteamVR_TrackedObject trackedObj;

	HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();

	private InteractableItem closestItem;
	private InteractableItem interactingItem;

	private bool isInTrigger = false;
	private bool isGripPressed = false;
	private bool isInPlatform = false;
	private bool targetOneReached = false;
	private bool targetTwoReached = false;
    private bool targetThreeReached = false;
	public bool istouchpadDown = false;
    public ushort gripVelocity = 5;
	Vector3 startpos;
    AudioSource audio;
	void Start () {
        Debug.Log("STAAAAAART");
		trackedObj = GetComponent<SteamVR_TrackedObject>();
        audio = GetComponent<AudioSource>();
	}

	void Update () {

		if (controller == null) {
			Debug.Log("Controller not initialized");
			return;
		}

		if (controller.GetPressDown (touchpadButton)) {
			Debug.Log("touch pad down");
			istouchpadDown = true;
			if (interactingItem != null) {
				Debug.Log ("reset entered");
				interactingItem.transform.position = startpos;
			
			}
		}

		if (controller.GetPressUp (touchpadButton)) {
			Debug.Log("touch pad up");
			istouchpadDown = false;
		}

		if (controller.GetPressDown (gripButton)) {
			isInTrigger = true;
			isGripPressed = true;
			if (interactingItem != null) {
				interactingItem.BeginInteraction(this);
			}
		}

		if (controller.GetPressUp(gripButton)) {
			isInTrigger = false;
			isGripPressed = false;
			if (interactingItem != null) {
				interactingItem.EndInteraction(this);
				objectsHoveringOver.Remove(interactingItem);
			}
		}
		if (isGripPressed) {
			controller.TriggerHapticPulse(gripVelocity);
		}

		if (controller.GetPressDown(triggerButton) && isInTrigger) {
			float minDistance = float.MaxValue;

			float distance;
			foreach (InteractableItem item in objectsHoveringOver) {
				distance = (item.transform.position - transform.position).sqrMagnitude;

				if (distance < minDistance) {
					minDistance = distance;
					closestItem = item;
				}
			}

			interactingItem = closestItem;

			if (interactingItem) {
				if (interactingItem.IsInteracting()) {
					interactingItem.EndInteraction(this);
				}

				interactingItem.BeginInteraction(this);
			}
		}

		if (controller.GetPressUp(triggerButton) && interactingItem != null) {
			interactingItem.EndInteraction(this);
			objectsHoveringOver.Remove(interactingItem);
		}
	}


	private void OnTriggerEnter(Collider collider) {
		if (collider.tag == "Platform") {
            audio.Play();
           // controller.TriggerHapticPulse(500);
			isInPlatform = true;
		} else if(collider.tag == "TargetOne"){
			targetOneReached = true;
		}else if(collider.tag == "TargetTwo"){
			targetTwoReached = true;
		}else if(collider.tag == "TargetTwo"){
			targetTwoReached = true;
        }else if (collider.tag == "TargetThree"){
           
            targetThreeReached = true;
        }
        else
        {
			InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
			isInTrigger = true;
			if (collidedItem) {
				objectsHoveringOver.Add(collidedItem);
			}
		}
	}

    public void increaseGrip(ushort value)
    {
        gripVelocity = value;
    }

	private void OnTriggerExit(Collider collider) {
		if (collider.tag == "Platform") {
			Debug.Log ("platform triggerrrrr exit");
           
			isInPlatform = false;
		} else if(collider.tag == "Target"){
			//targetReached = false;
		} else {
			InteractableItem collidedItem = collider.GetComponent<InteractableItem> ();
			isInTrigger = false;
			if (collidedItem) {
				objectsHoveringOver.Remove (collidedItem);
			}
		}
	}

	public bool getIsInTrigger(){
		return isInTrigger;
	}

	public bool getIsGripPressed(){
		return isGripPressed;
	}

	public bool getIsInPlatform(){
		return isInPlatform;
	}

	public bool getTargetOneReached(){
		return targetOneReached;
	}
	public bool getTargetTwoReached(){
		return targetTwoReached;
	}
    public bool getTargetThreeReached(){
        return targetThreeReached;
    }

    public void reset()
    {
        targetOneReached = false;
        targetTwoReached = false;
        targetThreeReached = false;

    }
}