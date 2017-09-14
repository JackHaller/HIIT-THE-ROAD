using UnityEngine;
using System.Collections;
using System;

public class BlockBuilder : MonoBehaviour {
	
	public Transform LeftFar;
	public Transform LeftClose;
	public Transform RightFar;
	public Transform RightClose;
	public Transform CentreBeam;
	public Transform RightWall;
	public Transform LeftWall;
	public Transform BarrierBar;
	public Transform BarrierSuppL;
	public Transform BarrierSuppR;
    public Transform PitAvoidanceTrigger;
    public Transform BarrierAvoidanceTrigger;
	
	public Transform SandpitR;	//Sand to fill pit on the right OR CENTRE (We never have a centre pit and a side pit on the same block)
	public Transform SandpitL;	//Sand to fill pit on the left only
	
	public static readonly int LEFT = 0;
	public static readonly int RIGHT = 1;
	public static readonly int BOTH = 2;
	public static readonly int CENTRE = 3;
	
	public void MakePit(int side, float length, float width, bool sand) 
    {
		if (side != LEFT && side != RIGHT && side != BOTH && side != CENTRE) {
			throw new ArgumentOutOfRangeException("side");		
		}
		float movementX = 0.25f * length;
		float movementZ = 0.5f * (4.0f - width);
		if (side == BOTH) {
			//enable the centre beam
			CentreBeam.GetComponent<Collider>().enabled = true;
			CentreBeam.GetComponent<Renderer>().enabled = true;
			//set the size of the beam
			CentreBeam.localScale = new Vector3(20.0f, 1.0f, 8.0f - (2.0f * width));
			//set the size of the side blocks
			Vector3 sideScale = new Vector3(10.0f - (0.5f * length), 1.0f, width);
			LeftFar.localScale = sideScale;
			LeftClose.localScale = sideScale;
			RightFar.localScale = sideScale;
			RightClose.localScale = sideScale;
			//set the positions of the side blocks
			LeftFar.position = new Vector3(LeftFar.position.x + movementX, LeftFar.position.y, LeftFar.position.z + movementZ);
			LeftClose.position = new Vector3(LeftClose.position.x - movementX, LeftClose.position.y, LeftClose.position.z + movementZ);
			RightFar.position = new Vector3(RightFar.position.x + movementX, RightFar.position.y, RightFar.position.z - movementZ);
			RightClose.position = new Vector3(RightClose.position.x - movementX, RightClose.position.y, RightClose.position.z - movementZ);
			//add sand if required
			if (sand) 
            {
				Vector3 sandScale = new Vector3(length, 1.0f, width);
				Vector3 sandPositionL = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 4.0f - (0.5f * width));
				Vector3 sandPositionR = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 4.0f + (0.5f * width));
				SandpitL.localScale = sandScale;
				SandpitL.transform.position = sandPositionL;
				SandpitL.GetComponentInChildren<MeshRenderer>().enabled = true;
				SandpitR.localScale = sandScale;
				SandpitR.transform.position = sandPositionR;
				SandpitR.GetComponentInChildren<MeshRenderer>().enabled = true;
			}
			
		} else if (side == CENTRE) {
			//We adjust the z scale of all side blocks (Note that this means that a centre pit will always be the entire length of the block)
			Vector3 sideScale = new Vector3(10.0f, 1.0f, 4.0f - (0.5f * width));
			LeftFar.localScale = sideScale;
			LeftClose.localScale = sideScale;
			RightFar.localScale = sideScale;
			RightClose.localScale = sideScale;
			//we move the side blocks outwards by 1/4 of the pit width
			float movement = 0.25f * width;
			LeftFar.transform.position = new Vector3(LeftFar.transform.position.x, LeftFar.transform.position.y, LeftFar.transform.position.z + movement);
			LeftClose.transform.position = new Vector3(LeftClose.transform.position.x, LeftClose.transform.position.y, LeftClose.transform.position.z + movement);
			RightFar.transform.position = new Vector3(RightFar.transform.position.x, RightFar.transform.position.y, RightFar.transform.position.z - movement);
			RightClose.transform.position = new Vector3(RightClose.transform.position.x, RightClose.transform.position.y, RightClose.transform.position.z - movement);
			//if we have sand, place it and make it visible
			if (sand) {
				Vector3 sandScale = new Vector3(20.0f, 1.0f, width);
				SandpitR.localScale = sandScale;
				SandpitR.transform.position = this.gameObject.transform.position;
				SandpitR.GetComponentInChildren<MeshRenderer>().enabled = true;
			}
			
		} else {
			//We have a pit on one side
			//we reduce the length of near and far by half the pit size, then move them outwards by half of that
			//Then we narrow the non pit side and widen the pit side
			Vector3 pitSideScale = new Vector3(10.0f - (0.5f * length), 1.0f, width);
			Vector3 baseSideScale = new Vector3(10.0f, 1.0f, 8.0f - width);
			Vector3 sandScale = new Vector3(length, 1.0f, width);
			if (side == LEFT) {
				//Adjust the scale of the left and right sections
				LeftFar.localScale = pitSideScale;
				LeftClose.localScale = pitSideScale;
				RightFar.localScale = baseSideScale;
				RightClose.localScale = baseSideScale;
				//adjust the position of the left (pit) sections
				LeftFar.position = new Vector3(LeftFar.position.x + movementX, LeftFar.position.y, LeftFar.position.z + movementZ);
				LeftClose.position = new Vector3(LeftClose.position.x - movementX, LeftClose.position.y, LeftClose.position.z + movementZ);
				//adjust the position of the right (base) sections
				RightFar.position = new Vector3(RightFar.position.x, RightFar.position.y, RightFar.position.z + movementZ);
				RightClose.position = new Vector3(RightClose.position.x, RightClose.position.y, RightClose.position.z + movementZ);
				//if we are pitting sand in the pits, adjust it now
				if (sand) {
					Vector3 sandPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 4.0f - (0.5f * width));
					SandpitL.localScale = sandScale;
					SandpitL.transform.position = sandPosition;
					var renderer = SandpitL.GetComponentInChildren<MeshRenderer>();
					renderer.enabled = true;
				}
			} else if (side == RIGHT) {
				//Adjust the scale of the left and right sections
				RightFar.localScale = pitSideScale;
				RightClose.localScale = pitSideScale;
				LeftFar.localScale = baseSideScale;
				LeftClose.localScale = baseSideScale;
				//Adjust the position of the right (pit) sections
				RightFar.position = new Vector3(RightFar.position.x + movementX, RightFar.position.y, RightFar.position.z - movementZ);
				RightClose.position = new Vector3(RightClose.position.x - movementX, RightClose.position.y, RightClose.position.z - movementZ);
				//Adjust the position of the left (base) sections
				LeftFar.position = new Vector3(LeftFar.position.x, LeftFar.position.y, LeftFar.position.z - movementZ);
				LeftClose.position = new Vector3(LeftClose.position.x, LeftClose.position.y, LeftClose.position.z - movementZ);
				//if we are pitting sand in the pits, adjust it now
				if (sand) {
					Vector3 sandPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 4.0f + (0.5f * width));
					SandpitR.localScale = sandScale;
					SandpitR.transform.position = sandPosition;
					SandpitR.GetComponentInChildren<MeshRenderer>().enabled = true;
				}
			}
		}
        //Set the position of the pit avoidance trigger plane to just after the end of the pit
        PitAvoidanceTrigger.localPosition = new Vector3(length / 2.0f, 1.5f, 0.0f);
	}
	
	public void RemoveWall(int side) {
		if (side == LEFT || side == BOTH) {
			LeftWall.GetComponent<Collider>().enabled = false;
			LeftWall.GetComponent<Renderer>().enabled = false;
		}
		if (side == RIGHT || side == BOTH) {
			RightWall.GetComponent<Collider>().enabled = false;
			RightWall.GetComponent<Renderer>().enabled = false;
		}
	}
	
	public void EnableBarrier() {
		BarrierBar.GetComponent<Collider>().enabled = true;
		BarrierSuppL.GetComponent<Collider>().enabled = true;
		BarrierSuppR.GetComponent<Collider>().enabled = true;

		BarrierBar.GetComponent<Renderer>().enabled = true;
		BarrierSuppL.GetComponent<Renderer>().enabled = true;
		BarrierSuppR.GetComponent<Renderer>().enabled = true;

        BarrierAvoidanceTrigger.GetComponent<Collider>().enabled = true;
	}
}
