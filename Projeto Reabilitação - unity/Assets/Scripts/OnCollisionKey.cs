using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using System;
using System.Runtime.InteropServices;
using System.IO;


public class OnCollisionKey : MonoBehaviour {

    /*Ler o nome do usuário atual*/
    public static string path;
    private string[] userName;
    private string[] users;
    private string[] performance;

    private static bool DoorTouch = false;

    public GameObject chave;


    /* Textos */
    public Text msg, msgCongrac, nameU;
    


    // Use this for initialization
    void Start () {
        users = new string[2];
        userName = new string[2];
        performance = new string[1];

        try
        {
            path = "./Files/UserInfo.txt";
            Debug.Log("Arquivo aberto com sucesso!");
        }
        catch
        {
            Debug.Log("Erro ao abrir o arquivo!");
        }
        userName = File.ReadAllLines(path);

        nameU.text = "Bem-vindo(a):" + userName[0];

        msg.text = "Leve a chave até a fechadura";

    }

	void Awake(){
		
	}

	// Update is called once per frame
	void Update () {
					
	}

    /*
	************************************************************************************
	 Ao detectar a colisão (entre o cursor e a trajetoria) o algoritmo 
	 identifica o objeto de colisão e processa a informação 
	************************************************************************************
	*/
    void OnCollisionEnter(Collision info)
    {
        /* Identifica se a chave colidiu com a fechadura */
        if (info.gameObject.name == "MarkerDoor")
        {
            Vector3 newPositionKey = new Vector3(-0.2197899f, 3.0651f, -5.2658f);
            chave.gameObject.transform.localPosition = newPositionKey;

            DoorTouch = true;
            Debug.Log ("Begin");
            msgCongrac.text = "Parabéns!";
            //espera 3 segundos para mudar de fase
            Invoke("MudarFase", 2);
        }
    }
    /*************************************************************************************/
    /* Muda a Fase */
    /*************************************************************************************/
    void MudarFase()
    {
        SceneManager.LoadScene("scenes/Fim");
    }

    public void SavePerformanceInformation() {

		try{            
			path = "./Files/key.txt";
			Debug.Log("Arquivo injection aberto com sucesso!");
		}
		catch {
			Debug.Log("Erro ao abrir o arquivo!");
		}

		if (DoorTouch == true){
			performance [0] = "true";
		}else {
			performance [0] = "false";
		}

		File.WriteAllLines(path, performance);
	}
}
