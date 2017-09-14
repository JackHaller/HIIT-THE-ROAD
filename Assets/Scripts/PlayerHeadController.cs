using UnityEngine;
using System.Collections;

public class PlayerHeadController : MonoBehaviour {

	public GlobalSettings globalSettings;

	public bool attached = true;
    //public bool usingOculus = false;
	public float remainingLife = 1.0f;
	
	public PlayerController player;
	public KinectController kinect;
    public PipeSystemController camera3D;
	public GameObject followCamera;
	//public GameObject oculusCamera;
	public bool SpectatorMode;
	public Transform currentFocus;
	public PlaybackManager playback;
	
	private bool saveOriginalCam;
    private bool birdsEye = false;
    private Transform originalCameraRotPOS;
	
	private Generator generator;
	
	void Start() {
		//usingOculus = globalSettings.UseOculus;
		this.generator = GameObject.Find("Generator").GetComponent<Generator>();	
	}
	
	void Update() {
		if (!attached) 
        {
			if (remainingLife <= 0.0f) 
            {
				if (player.lives > 0) 
                {
					player.Respawn();
				} 
                else 
                {
					player.DoGameOver();	
				}
				attached = true;
				remainingLife = 3.0f;
				this.GetComponent<Rigidbody>().useGravity = false;
				this.GetComponent<Collider>().isTrigger = true;
			} 
            else 
            {
				remainingLife -= Time.deltaTime;
			}
		} 
        else 
        {
			//head is attached, keep following that player
			Vector3 headPositionOffset = new Vector3(0.0f, 0.0f, 0.0f);		//Later should be able to pull this completely from kinect
			if (kinect.EnableKinect && kinect.calibrationFinished) 
            {
                headPositionOffset = (kinect.currentHeadPosition - kinect.defaultheadPosition);
				headPositionOffset.y *= -1.0f;
			}
            else if (camera3D.EnableCamera && camera3D.CameraInitialised)
            {
                //we use simpler tracking with the 3d camera
                if (camera3D.PositionOffset.y < 0)
                {
                    this.transform.position = new Vector3(player.transform.position.x + 0.45f, player.transform.position.y + 0.25f, player.transform.position.z) - headPositionOffset;
                }
                else
                {
                    this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.75f, player.transform.position.z) - headPositionOffset;
                }
            }
			else if(Input.GetKey(KeyCode.Space)) //we can also duck with the spacebar (when testing)
            {
				this.transform.position = new Vector3(player.transform.position.x+0.45f, player.transform.position.y + 0.25f, player.transform.position.z) - headPositionOffset;
			}
            else
            {
				this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.75f, player.transform.position.z) - headPositionOffset;
			}
		}
		if(SpectatorMode)
        {
            if (currentFocus == null)
            {
                currentFocus = playback.NextGhost(); // set it up.
            }
			if(Input.GetKeyDown(KeyCode.O))
            {
				currentFocus = playback.NextGhost();
			}
			if(Input.GetKeyDown(KeyCode.P))
            {
				birdsEye = !birdsEye;
                if (birdsEye)
                {
                    followCamera.GetComponent<Camera>().fieldOfView = 100;
                }
                if (!birdsEye)
                {
                    followCamera.GetComponent<Camera>().fieldOfView = 60;
                }
			}
			
		}
	}

	void LateUpdate() 
    {
		if(!SpectatorMode)
        {
			followCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
			//oculusCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);	
		}
        else
        {
			player.GetComponent<Renderer>().enabled  = false; // disable the player render;
			this.GetComponent<Renderer>().enabled = false; // disable the head render;
			player.canDie = false; // Make him invincible.
			player.generator.HijackBlockBuildThreshold(); // Noice.
			if(currentFocus != null)
            {
				//player.gui.UpdateOculusUI(currentFocus.GetChild(1).position);
				if(birdsEye)
                {
					followCamera.transform.position = new Vector3(currentFocus.GetChild(1).transform.position.x, currentFocus.GetChild(1).transform.position.y + 15f,
						currentFocus.GetChild(1).transform.position.z);
					followCamera.transform.LookAt(currentFocus.GetChild(1).position);
				}
                else
                {
					followCamera.transform.position = new Vector3(currentFocus.GetChild(1).transform.position.x - 4.25f, currentFocus.GetChild(1).transform.position.y + 2.75f,
						currentFocus.GetChild(1).transform.position.z);
					followCamera.transform.LookAt(currentFocus.GetChild(1).position);
					//followCamera.transform.Rotate(new Vector3(15,90,0));
					followCamera.transform.eulerAngles = (new Vector3(15,90,0)); // Fixes the camera rotation after using look at.
				}
			}
            else
            {
				Debug.LogError("No current camera focus in spectator mode. ALl ghosts probably dead. Killing Spec mode.");
				SpectatorMode = false;
				
			}
		}
	}
	
	void OnTriggerEnter(Collider other) 
    {
		if (other.tag == "bar") 
        {
			//We just hit something
			if (generator.USE_FATAL_OVERHEAD) {
				attached = false;
				this.GetComponent<Rigidbody>().useGravity = true;
				this.GetComponent<Collider>().isTrigger = false;
				player.canDie = false;
			}
			else 
            {
				player.BeginEnvironmentalResistanceOverride(15);	
			}
		}
	}
	
	void OnTriggerExit(Collider other) 
    {
		if (other.tag == "bar" && !generator.USE_FATAL_OVERHEAD) {
			player.EndEnvironmentalResistanceOverride();	
		}
	}
}
