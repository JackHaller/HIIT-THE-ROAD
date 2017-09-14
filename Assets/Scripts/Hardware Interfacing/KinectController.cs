//Handles retrieval of input from the Kinect in order to track the player's head location

using UnityEngine;
using System;
using System.Collections;
using System.Threading;

public class KinectController : MonoBehaviour {

    public GlobalSettings globalSettings;

	public bool EnableKinect = true;
	
	public bool ending = false;
	
	private int calibrationFrames = 0;
	public int FRAMES_TO_CALIBRATE = 200;
	public bool calibrationFinished = false;
	
	private Thread trackerThread;
	
	public Vector3 defaultheadPosition = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 currentHeadPosition = new Vector3(0.0f, 0.0f, 0.0f);
	
	public float movement {
		get{
			return (currentHeadPosition.z - defaultheadPosition.z) * 3.0f;
		}
	}
	
	public float ducking {
		get {
			return (currentHeadPosition.x - defaultheadPosition.x);	
		}
	}
	
	public SkeletonWrapper sw;
	
	public int player;
	public BoneMask Mask = BoneMask.All;
	
	public float scale = 1.0f;
	
	// Use this for initialization
	void Start () {
        EnableKinect = globalSettings.EnableKinect;
		Initialise();
	}
	
	public void Initialise() {
		if (EnableKinect) {
			ending = false;
			calibrationFrames = 0;
			calibrationFinished = false;
			trackerThread = new Thread(ThreadMethod);
			trackerThread.Start();
		} else {
			calibrationFinished = true;	//Set this to auto-true so we can move	
		}
	}
	
	//Needs some tidy up
	void ThreadMethod() {
		Debug.Log("Kinect Thread Started");
		while (ending == false) {
			if(player != -1) {
				//update all of the bones positions
				Thread.Sleep(50);
				if (sw.setupFinished && sw.pollSkeleton())
				{
					for( int ii = 0; ii < (int)Kinect.NuiSkeletonPositionIndex.Count; ii++) {
						if( ((uint)Mask & (uint)(1 << ii) ) > 0 ){		//really don't need both of these ifs
							if( ((uint)(0x8) & (uint)(1 << ii) ) > 0 ){
								currentHeadPosition.x = sw.bonePos[player,ii].x;
								currentHeadPosition.y = sw.bonePos[player,ii].y;
								currentHeadPosition.z = sw.bonePos[player,ii].z;
								//Debug.Log (currentHeadPosition);
								if (!calibrationFinished) {
									calibrationFrames++;
									//Debug.Log (calibrationFrames);
									if (calibrationFrames >= FRAMES_TO_CALIBRATE) {
										calibrationFinished = true;
										defaultheadPosition.x = currentHeadPosition.x;
										defaultheadPosition.y = currentHeadPosition.y;
										defaultheadPosition.z = currentHeadPosition.z;
										Debug.Log ("Calibration Finished");
									}
								}
							}
						}
					}
				}
			}
		}
	}
	
	void OnApplicationQuit() {
		ending = true;	
	}
	
	//Assignments for a bitmask to control which bones to look at and which to ignore
	public enum BoneMask
	{
		None = 0x0,
		Hip_Center = 0x1,
		Spine = 0x2,
		Shoulder_Center = 0x4,
		Head = 0x8,					//At this stage, the only bone we actually care about tracking is the head
		Shoulder_Left = 0x10,
		Elbow_Left = 0x20,
		Wrist_Left = 0x40,
		Hand_Left = 0x80,
		Shoulder_Right = 0x100,
		Elbow_Right = 0x200,
		Wrist_Right = 0x400,
		Hand_Right = 0x800,
		Hip_Left = 0x1000,
		Knee_Left = 0x2000,
		Ankle_Left = 0x4000,
		Foot_Left = 0x8000,
		Hip_Right = 0x10000,
		Knee_Right = 0x20000,
		Ankle_Right = 0x40000,
		Foot_Right = 0x80000,
		All = 0xFFFFF,
		Torso = 0x10000F, //the leading bit is used to force the ordering in the editor
		Left_Arm = 0x1000F0,
		Right_Arm = 0x100F00,
		Left_Leg = 0x10F000,
		Right_Leg = 0x1F0000,
		R_Arm_Chest = Right_Arm | Spine,
		No_Feet = All & ~(Foot_Left | Foot_Right),
		UpperBody = Shoulder_Center | Head|Shoulder_Left | Elbow_Left | Wrist_Left | Hand_Left|
		Shoulder_Right | Elbow_Right | Wrist_Right | Hand_Right	
	}
}
