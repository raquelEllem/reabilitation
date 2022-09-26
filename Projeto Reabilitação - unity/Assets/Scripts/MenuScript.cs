using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

public class MenuScript : MonoBehaviour {

	public HandCollisionFase11 o11;
	public HandCollisionFase12 o12;

	//public Button Linhas;
	// Use this for initialization
	void Start () {
		Debug.Log ("Menu");	
		o11 = gameObject.AddComponent<HandCollisionFase11> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CenasLinhas(){		
		/* Carrega a primeira cena da Fase */
		SceneManager.LoadScene ("scenes/Fase11");
	}

	

	public void CenasTarefas(){
		SceneManager.LoadScene ("scenes/PainelCores");
	}



}
