using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;

//Stores information about a block of the Track

public interface IBlock {
	void DestroyBlock();
	Vector3 GetSpawnPosition(Vector3 playerPosition);
	void AddSpawnPlatform(Transform platform);
	string GetInfo(Vector3 playerPosition);
}

[Serializable]
public class Block : IBlock {

	public string BlockInfo = string.Empty;

	public Transform TrackSection = null;

	public Transform Cannon = null;

	public Transform Powerup = null;

	public Transform SpawnPlatform = null;
	
	
	public Block(Transform track) {
		TrackSection = track;
		Cannon = null;
		Powerup = null;
	}
	
	public Block(){
	}
	
	public Block(Transform track, Transform cannon, Transform powerup) {
		TrackSection = track;
		Cannon = cannon;
		Powerup = powerup;
	}
	
	public void DestroyBlock() {
		if (Cannon != null) {
			UnityEngine.Object.Destroy(Cannon.gameObject);
		}
		if (TrackSection != null) {
			UnityEngine.Object.Destroy(TrackSection.gameObject);
		}
		if (Powerup != null) {
			UnityEngine.Object.Destroy(Powerup.gameObject);	
		}
		if (SpawnPlatform != null) {
			Debug.Log ("Destroying spawn platform");
			UnityEngine.Object.Destroy(SpawnPlatform.gameObject);	
		}
	}

	public Vector3 GetSpawnPosition(Vector3 playerPosition) {
		if (TrackSection != null) {
			float heightMod = 1.0f;
			return new Vector3(TrackSection.position.x, TrackSection.position.y + heightMod, TrackSection.position.z);
		} else {
			throw new InvalidOperationException();	
		}
	}
	
	public  void AddSpawnPlatform(Transform platform) {
		this.SpawnPlatform = platform;	
	}
	
	public  string GetInfo(Vector3 playerPosition) {
		return BlockInfo;
	}
}

public class TwinnedBlock : IBlock {
	
	public Block sectionL = null;
	public Block sectionR = null;
	
	public void DestroyBlock() {
		if (sectionL != null) {
			sectionL.DestroyBlock();
		}
		if (sectionR != null) {
			sectionR.DestroyBlock();	
		}
	}
	
	public  Vector3 GetSpawnPosition(Vector3 playerPosition) {
		if (sectionL == null || sectionR == null) {
			throw new InvalidOperationException();	
		}
		if (playerPosition.z < 0) {
			//Player is in the right section
			return sectionR.GetSpawnPosition(playerPosition);
		} else {
			//Player is in the left section
			return sectionL.GetSpawnPosition(playerPosition);
		}
	}
	
	public  void AddSpawnPlatform(Transform platform) {
		if (sectionL == null || sectionR == null) {
			throw new InvalidOperationException();	
		}
		if (platform.position.z < 0) {
			sectionR.AddSpawnPlatform(platform);	
		} else {
			sectionL.AddSpawnPlatform(platform);
		}
	}
	
	public  string GetInfo(Vector3 playerPosition) {
		if (sectionL == null || sectionR == null) {
			throw new InvalidOperationException();	
		}
		if (playerPosition.z < 0) {
			//Player is in the right section
			return sectionR.GetInfo(playerPosition);
		} else {
			//Player is in the left section
			return sectionL.GetInfo(playerPosition);
		}
	}
}