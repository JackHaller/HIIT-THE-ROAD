using System;
using System.IO;
using UnityEngine;

namespace Exergame
{
	public class BlockSerializer
	{
		private StreamWriter myWriter;
		public bool isUsable = false;
		
		public BlockSerializer ()
		{
			//Let's open our file.
			myWriter = new StreamWriter(File.Open(System.Environment.CurrentDirectory + "/WorldPlaybackData/Recording.egw",FileMode.OpenOrCreate));
			isUsable = true;
			WriteDataHeader();
		}
		
		private void WriteDataHeader(){
			myWriter.WriteLine("Timestamp: " + System.DateTime.Now.ToString("dd MMMM yyyy hh:mm:ss"));

		}
		
		public void WriteTutorialData(CircularLinkedList<IBlock> blocks){
			//All tutorial parts are just simple blocks.
			LinkedListNode<IBlock> temp = blocks.Tail;
			do{
				Serialize((Block)temp.Value);
				temp = temp.Previous;
			}while(temp != blocks.Tail);
		}
		
		public void Serialize(IBlock b){
			if(b is TwinnedBlock){
				ProcessTwinnedBlock((TwinnedBlock) b);
			}else if(b is Block){
				SerializeBlock((Block) b);
			}else{
			Debug.LogError("Cannot detect type in Block Serializer.");	
			}
		}
		//The common block serialize code. Put everything we need to store to disk in here!.
		private void SerializeBlock (Block t)
		{
			myWriter.WriteLine("<block " + t.BlockInfo +">");
			if(t.TrackSection.name != null) myWriter.WriteLine("<sectiontype>" +t.TrackSection.name + "/>"); // This returns the blocktype! Null if branched?
			if(t.Cannon != null) myWriter.WriteLine("<CannonPosXYZ:"+t.Cannon.position.x + ":" + t.Cannon.position.y + ":" + t.Cannon.position.z + ":</>");
			if(t.Powerup != null) myWriter.WriteLine("<PowerPosXYZ:"+t.Powerup.position.x +":"+ t.Powerup.position.y +":"+ t.Powerup.position.z +":</>");
			myWriter.WriteLine("<BlockPosXYZ>:" +t.TrackSection.position.x + ":" + t.TrackSection.position.y + ":" + t.TrackSection.position.z + ":</>");
			myWriter.WriteLine("</block " + t.BlockInfo + ">");
		}
		//As TwinnedBlocks are just two Blocks together, we just need to process them separately and store them inside a twinnedblocksection.
		private void ProcessTwinnedBlock(TwinnedBlock b){
			//Debug.LogWarning("TwinnedBlockProcessing");
			myWriter.WriteLine("<TwinnedBlock>");
			myWriter.WriteLine("<LEFT>");
			SerializeBlock(b.sectionL);
			myWriter.WriteLine("</LEFT>");
			myWriter.WriteLine("<RIGHT>");
			SerializeBlock(b.sectionR);
			myWriter.WriteLine("</RIGHT>");
			myWriter.WriteLine("</TwinnedBlock>");
		}
		
		public void CloseBlockRecorder(){
			myWriter.Close();
			isUsable = false;
		}
	}
}

