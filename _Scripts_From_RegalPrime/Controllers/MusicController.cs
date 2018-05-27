// RegalPrime 12-02-14 - MusicController.cs

// There can only be one of these objects in a scene at a time
// If a new object is found (or loaded), they compare their musicclips and if they are different, it loads and plays the new one, then destroys itself

// Used to play both music / effects
// Has options to change music / fade / unfade
// Used to save preferences to a file "GameSettings.dat" This file will be created if not available


using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MusicController : MonoBehaviour
{
	public static MusicController control;			// Reference to itself

	public AudioClip musicclip;						// Audio that place when entering a scene
	AudioSource audioSource;						// Main Audiosource - It is added to the object via script

	private float fade_speed = 5;					// Fade speed
	private float unfade_speed = 5;					// Unfade speed
	private float maxVolume;						// Used in fade / unfade of music

	public float musicVolume = 1f;					// Current music volume
	public float effectVolume = 1f;					// Current effect volume

	void Awake ()
	{
		audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.loop = true;

		if (control == null)						// If there are no control objects (this is the main one)
		{
			// This stuff will be saved in a file later and called from various functions /////
			Load ();

			maxVolume = musicVolume;
			audioSource.volume = musicVolume;
			///////////////////////////////////////////////////////////////////////////////////

			PlayNewClip(musicclip);					// Play the musicclip

			DontDestroyOnLoad (gameObject);			// Persist
			control = this;							// Reference itself
			gameObject.transform.parent = null;
		}
		else if(control != this)					// If there is already a control object 
		{
			if(musicclip != null)					// If there is music on this MusicController
			{
				if(MusicController.control.musicclip == null)				// If main controller has null music, put it on the music controller that exists
					MusicController.control.PlayNewClip(musicclip);
				else

				if(musicclip.name != MusicController.control.audioSource.clip.name) // If this is new music - put it on the music controller that exists
					MusicController.control.Fade_Play_Unfade(musicclip);
				else
					MusicController.control.UnFadeMusic ();

			}
			Destroy(gameObject);	// Information has been sent to the main MusicControl object - delete the duplicate
		}
	}

	public void PlayNewClip(AudioClip newAudioClip)	// Adds and plays the new music
	{	
		if(audioSource.clip != newAudioClip)
		{
			audioSource.Stop ();
			musicclip = newAudioClip;
			audioSource.clip = newAudioClip;
			audioSource.Play ();
		}
	}

	public void SkipToPoint(float value)
	{
		print ("Skip to : " + value + " / " + audioSource.clip.length + " - Total Length");
		audioSource.time = value;
	}

	public void Set_LoopSong(bool shouldILoop)
	{
		audioSource.loop = shouldILoop;
	}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
	// An alternate version of unitys oneshotaudtio
	// Plays the effect at the prefered sound and returns a reference
	public AudioSource OneShotAudio(AudioClip newAudioClip)
	{
		GameObject TEMPobject = new GameObject("OneShotAudio");
		TEMPobject.transform.position = gameObject.transform.position;
		
		AudioSource TEMPaudiosource = TEMPobject.AddComponent<AudioSource>();
		
		TEMPaudiosource.volume = effectVolume;
		TEMPaudiosource.clip = newAudioClip;
		TEMPaudiosource.Play();
		
		Destroy (TEMPobject, newAudioClip.length);

		return TEMPaudiosource;
	}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
	public void FadeMusic()
	{
		StopAllCoroutines ();
		StartCoroutine("C_FadeMusic");
	}
	IEnumerator C_FadeMusic()					// Coroutine that fades the music over time
	{
		while(audioSource.volume > .1F)
		{
			audioSource.volume = Mathf.Lerp(audioSource.volume,0F,Time.deltaTime*fade_speed);
			yield return null;
		}
		audioSource.volume = 0;
	}

	public void UnFadeMusic ()
	{
		StopAllCoroutines ();
		StartCoroutine("C_UnFadeMusic");
	}
	IEnumerator C_UnFadeMusic()				// Coroutine that unfades the music over time
	{
		while(audioSource.volume < maxVolume-.1)
		{
			audioSource.volume = Mathf.Lerp(audioSource.volume, maxVolume,Time.deltaTime*unfade_speed);
			yield return null;
		}
		audioSource.volume = maxVolume;
	}

	// Takes a clip of music as a parameter
	// then calls a coroutine that add that clip to the musiccontroller and does a fade / change clip / unfade effect
	public void Fade_Play_Unfade(AudioClip TEMPCLIP)
	{
		StopAllCoroutines ();

		musicclip = TEMPCLIP;
		StartCoroutine("C_Fade_Play_Unfade");
	}
	IEnumerator C_Fade_Play_Unfade()
	{
		while(audioSource.volume > .1F)
		{
			audioSource.volume = Mathf.Lerp(audioSource.volume,0F,Time.deltaTime*fade_speed);
			yield return null;
		}
		audioSource.volume = 0;

		audioSource.Stop ();
		audioSource.clip = musicclip;
		audioSource.Play();

		while(audioSource.volume < maxVolume-.1)
		{
			audioSource.volume = Mathf.Lerp(audioSource.volume, maxVolume,Time.deltaTime*unfade_speed);
			yield return null;
		}
		audioSource.volume = maxVolume;
	}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


	// Save current preferences
	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create ("GameSettings"+".dat");

		Musicdata data = new Musicdata ();
		data.MusicVolume = musicVolume;
		data.EffectVolume = effectVolume;

		bf.Serialize (file, data);
		file.Close ();
	}
	
	// Load current preferences
	public void Load()
	{
		if(File.Exists("GameSettings"+".dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open ("GameSettings"+".dat", FileMode.Open);
			
			Musicdata data = (Musicdata)bf.Deserialize(file);
			file.Close();
			
			musicVolume = data.MusicVolume;
			effectVolume = data.EffectVolume;
		}
		else
		{
			musicVolume = 0.5f;
			effectVolume = 0.5f;
			Save ();
		}
	}

	// Change the volume to the current set variable
	public void RefreshVolume()
	{
		maxVolume = musicVolume;
		if(audioSource != null)
			audioSource.volume = musicVolume;
	}

	public bool AbleToLoadFile()
	{
		return File.Exists("GameSettings"+".dat");
	}
}

// Used to save the data in one object
[Serializable]
class Musicdata
{
	public float MusicVolume;
	public float EffectVolume;
}