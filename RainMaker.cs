//Author: God Bennett
//Extortionist 2024
//Some reverse engineering done to hook into VegetationSystemPro and Aura Lighting, to dynamically set wind and light respectively at runtime.

//switch between normal wind zone, and stormy windzone created by God

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies.VegetationSystem
{
	public class RainMaker : MonoBehaviour
	{
		public bool startWithRain = true;
		
		//general
		public GameObject RAIN_FX_FACTORY;

		
		//thunder/rain (local to player)
		public GameObject ThirdPersonController;
		public GameObject RainComponentsParent;
		private GameObject RainSystem;
		private GameObject ThunderSystem;
		private GameObject RainAudio;
		private GameObject RainCameraScreenFx;
		
		//road material
		public Material globalRoadMaterial;
		private Shader roadShaderDry;
        private Shader roadShaderWet;
		
		
		//ambience/nature - music
		public GameObject globalCameraAsObj;
		private GameObject GlobalNatureAmbienceMusic;
		
		
		//vegetation studio
		public WindZone WindZone_Normal;
		public WindZone WindZone_Storm;
		public VegetationSystemPro vsp;
		public enum WIND_MODES { NORMAL, STORMY };
		private WIND_MODES currentWindMode;
		
		
		//Aura stuff
		public Aura2API.AuraBaseSettings Aura_Extortionist_Normal;
		public Aura2API.AuraBaseSettings Aura_Extortionist_Stormy;
		public Camera globalCamera;
		private Aura2API.AuraCamera auraCamera;
		
		
		void Start ( )
		{
			/////////////////
			//setup wind stuff
			currentWindMode = WIND_MODES.NORMAL;
			
			
			/////////////////
			//setup light stuff
			//setup main aura cam, a script added to Main Standard Cam by God, to set night/day visual (enable/disable aura ambient light)
			auraCamera = globalCamera.GetComponent<Aura2API.AuraCamera>();
			
			/////////////////
			//setup rain stuff
			RainSystem = FindGameObjectInChildWithName ( RainComponentsParent, "--EXTRACT_REALISTIC_RAIN_STORM_SCENE" );
			
			ThunderSystem = FindGameObjectInChildWithName ( RainComponentsParent, "--THUNDER_LIGHTNING" );
			
			RainAudio = FindGameObjectInChildWithName ( RainComponentsParent, "--RAIN_AUDIO" );
			
			GlobalNatureAmbienceMusic = FindGameObjectInChildWithName ( globalCameraAsObj, "Music" );
			
			RainCameraScreenFx = FindGameObjectInChildWithName ( globalCameraAsObj, "--RAINFX_DOBBEY" );
			
			roadShaderDry =  Shader.Find("Standard");
			roadShaderWet =  Shader.Find("Ciconia Studio/CS_Polybrush/Builtin/Pro/Rainy Puddles");
			
			if ( startWithRain )
			{
				vsp.SelectedWindZone = WindZone_Storm;
				currentWindMode = WIND_MODES.STORMY;
				auraCamera.frustumSettings.baseSettings = Aura_Extortionist_Stormy;
				RainSystem.SetActive (true);
				ThunderSystem.SetActive (true);
				RainAudio.SetActive (true);
				GlobalNatureAmbienceMusic.SetActive (false);
				RainCameraScreenFx.SetActive (true);
				globalRoadMaterial.shader = roadShaderWet;
			}
		}
		
		// Update is called once per frame
		void Update()
		{
			RainComponentsParent.transform.position = ThirdPersonController.transform.position;
			 
			if (Input.GetButtonDown("RainStorm"))
			{
				if ( currentWindMode == WIND_MODES.NORMAL )
				{
					vsp.SelectedWindZone = WindZone_Storm;
					currentWindMode = WIND_MODES.STORMY;
					auraCamera.frustumSettings.baseSettings = Aura_Extortionist_Stormy;
					RainSystem.SetActive (true);
					ThunderSystem.SetActive (true);
					RainAudio.SetActive (true);
					GlobalNatureAmbienceMusic.SetActive (false);
					RainCameraScreenFx.SetActive (true);
					globalRoadMaterial.shader = roadShaderWet;
				}
				else if ( currentWindMode == WIND_MODES.STORMY )
				{
					vsp.SelectedWindZone = WindZone_Normal;
					currentWindMode = WIND_MODES.NORMAL;
					auraCamera.frustumSettings.baseSettings = Aura_Extortionist_Normal;
					RainSystem.SetActive (false);
					ThunderSystem.SetActive (false);
					RainAudio.SetActive (false);
					GlobalNatureAmbienceMusic.SetActive (true); //re-enable natural music
					RainCameraScreenFx.SetActive (false);
					globalRoadMaterial.shader = roadShaderDry;
				}
			}
		}
		
		public GameObject FindGameObjectInChildWithName (GameObject parent, string name)
		{
			GameObject returnValue = null;
			
			Transform t = parent.transform;

			for (int i = 0; i < t.childCount; i++) 
			{
				//Debug.Log (  t.GetChild(i).name );
				if(t.GetChild(i).name == name)
				{
					returnValue = t.GetChild(i).gameObject;
				}

			}

			return returnValue;
		}
		public GameObject FindGameObjectInChildWithNameFromTransform (Transform parentTransform, string name)
		{
			GameObject returnValue = null;
			
			Transform t = parentTransform;

			for (int i = 0; i < t.childCount; i++) 
			{
				//Debug.Log (  t.GetChild(i).name );
				if(t.GetChild(i).name == name)
				{
					returnValue = t.GetChild(i).gameObject;
				}

			}

			return returnValue;
		}
		public bool isRaining ( )
		{
			return currentWindMode == WIND_MODES.STORMY;
		}
		
		//Ensures that rain windshield/wiper is not enabled before driver gets in car
		public IEnumerator LateRainObjectsEnabling(float seconds, GameObject RainWindshieldObject, GameObject RainWindshieldFxObject, GameObject DefaultWindshield, GameObject WiperAudioObj)
		{
			yield return new WaitForSeconds(seconds);

			//re-boot the rain fx factory to reflect new rain windshield activity
			RAIN_FX_FACTORY.SetActive (false);
			RAIN_FX_FACTORY.SetActive (true);
				
			//enable windshield objects and default windshield
			RainWindshieldObject.SetActive (true);
			RainWindshieldFxObject.SetActive (true);
			WiperAudioObj.SetActive (true);
			DefaultWindshield.SetActive (false);
		}	
		
		public void disableRainWindshieldSystem(GameObject RainWindshieldObject, GameObject RainWindshieldFxObject, GameObject DefaultWindshield, GameObject WiperAudioObj)
		{
			if ( RainWindshieldObject != null )
			{
				//enable windshield objects and default windshield
				RainWindshieldObject.SetActive (false);
				RainWindshieldFxObject.SetActive (false);
				WiperAudioObj.SetActive (false);
				DefaultWindshield.SetActive (true);
			}
		}
	}
}
