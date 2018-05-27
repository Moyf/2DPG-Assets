// RegalPrime 12-20-14 - PreferencesGUI.cs

// This holds all of the button / functions calls / etc for the Preferences GUI section


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PreferencesGUI : MonoBehaviour
{
	public AudioClip effectClipTest;				// Effect that is previewed when changing the sound effect slidebar

	// References to particular GUI elements
	public Slider MusicSlider;
	public Text MusicSliderValue;
	public Slider EffectSlider;
	public Text EffectSliderValue;

	private float musicValueChange = 0;
	private float effectValueChange = 0;


	public Slider SceneSkipSlider;
	public Text SceneSkipTextValue;


	// Set the slider bar values
	void Start()
	{
		musicValueChange = (Mathf.Round (MusicController.control.musicVolume*10))/10;
		MusicSlider.value = musicValueChange;
		MusicSliderValue.text = ""+musicValueChange*10;

		effectValueChange = (Mathf.Round (MusicController.control.effectVolume*10))/10;
		EffectSlider.value = effectValueChange;
		EffectSliderValue.text = ""+effectValueChange*10;

		SceneSkipSlider.maxValue = Application.levelCount - 1;
		SceneSkipSlider.value = 0;
		SceneSkipTextValue.text = "0";
	}
	void OnDestroy()
	{
		MusicController.control.Load ();
		MusicController.control.RefreshVolume ();
	}

	// Fucntion to preview the music volume via the slide bar
	public void MusicSliderChange()
	{
		float musicTEMP = (Mathf.Round (MusicSlider.value * 10)) / 10;

		if(musicValueChange != musicTEMP)
		{
			MusicController.control.musicVolume = musicTEMP;
			MusicSliderValue.text = ""+musicTEMP*10;
			MusicController.control.RefreshVolume();

			musicValueChange = musicTEMP;
		}
	}
	// Fucntion to preview the sound effect volume via the slide bar
	public void EffectSliderChange()
	{
		float effectTEMP = (Mathf.Round (EffectSlider.value * 10)) / 10;

		if(effectValueChange != effectTEMP)
		{
			MusicController.control.effectVolume = effectTEMP;
			EffectSliderValue.text = ""+effectTEMP*10;
			MusicController.control.OneShotAudio(effectClipTest);

			effectValueChange = effectTEMP;
		}
	}

	// Function to save the current preferences of music / sound effects volume
	public void SaveSoundSettings()
	{
		MusicController.control.musicVolume = MusicSlider.value;
		MusicController.control.effectVolume = EffectSlider.value;
		MusicController.control.Save();
		MusicController.control.RefreshVolume();
	}

	// Used to do the scene change panel
	public void SceneSliderChange()
	{
		SceneSkipTextValue.text = ""+SceneSkipSlider.value;
	}
	public void JumpToScene()
	{
		Application.LoadLevel ((int)SceneSkipSlider.value);
	}

	// Link to forum post
	public void GotoLink()
	{
		Application.OpenURL ("http://www.iwannacommunity.com/forum/index.php?topic=1150.0");
	}
}