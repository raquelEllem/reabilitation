using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using System;
using System.Runtime.InteropServices;
using System.IO;

public class OnCollisionInjection : MonoBehaviour {

    public static string path;
	private string[] performance;

	public GameObject Syringe;

	private bool Marker1 = false, Marker2 = false, ProcedimentoOK = false;

	public Text  msg;

	// Use this for initialization
	void Start () {

        performance = new string[1];

		msg.text = "Aplique a injeção na marcação";

		Syringe.gameObject.GetComponent<Renderer> ().material.color = Color.gray;	

		try{            
			path = "./Files/injection.txt";
			Debug.Log("Arquivo injection aberto com sucesso!");
		}
		catch {
			Debug.Log("Erro ao abrir o arquivo!");
		}
	}

	void Awake(){

	}

	// Update is called once per frame
	void Update () {
		

    }

    void OnCollisionEnter(Collision info)
    {
        /* Identifica se a seringa colidiu com a marcação*/
        if (info.gameObject.name == "Marker1")
        {
            msg.text = "Aplicacao realizada com sucesso!";
            ProcedimentoOK = true;
            Marker1 = true;
            Syringe.gameObject.GetComponent<Renderer>().material.color = Color.green;
            msg.text = "Parabéns!";
            //espera 3 segundos para mudar de fase
            Invoke("MudarFase", 1);
        }
        if (Marker1 == true && info.gameObject.name == "Marker2")
        {
            msg.text = "Muito profundo! Retire a seringa e tente novamente!";
            ProcedimentoOK = false;
            Marker2 = true;
            Syringe.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        }
        if (Marker2 == true && info.gameObject.name == "Marker3")
        {
            msg.text = "Muito profundo! Retire a seringa e tente novamente!";
            ProcedimentoOK = false;
            Syringe.gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        
    }

    void MudarFase()
    {
        SceneManager.LoadScene("scenes/Fim");
    }

    public void SavePerformanceInformation()
    {

        try
        {
            path = "./Files/injection.txt";
            Debug.Log("Arquivo injection aberto com sucesso!");
        }
        catch
        {
            Debug.Log("Erro ao abrir o arquivo!");
        }

        if (ProcedimentoOK == true)
        {
            performance[0] = "true";
        }
        else
        {
            performance[0] = "false";
        }

        File.WriteAllLines(path, performance);
    }

}