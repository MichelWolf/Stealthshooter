using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour {

	public GameObject loadingScreen;
	public Slider loadingSlider;

	public void LoadLevel (int sceneIndex)
	{
		//Irgendwas nebenläufig laufendes
		StartCoroutine (LoadAsync(sceneIndex));
	}

	IEnumerator LoadAsync(int sceneIndex){
		//Lädt asyncron die Szene
		AsyncOperation operation = SceneManager.LoadSceneAsync (sceneIndex);
		loadingScreen.SetActive (true);
		//Solange das Laden noch nicht fertig ist
		while (!operation.isDone) {
			//rechne den prozentualen Progress aus
			float progress = Mathf.Clamp01 (operation.progress / 0.9f);
			//und setze den slider auf den Progresswert -> Ladebalken
			loadingSlider.value = progress;
			yield return null;
		}
	}
}
