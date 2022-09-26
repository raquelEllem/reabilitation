using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverObjeto : MonoBehaviour {

    public float velocidade = 10;
    public float velocidade2 = 2;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float moverX = Input.GetAxis("Horizontal");
        float moverY = Input.GetAxis("Vertical");
        bool moverZ = Input.GetKeyDown("z");

        Vector3 direcaoMovimento = new Vector3(moverX, moverY);
        transform.Translate(direcaoMovimento * velocidade * Time.deltaTime);

        if (Input.GetKey("w"))
        {
            transform.Translate(0, 0, (velocidade2 * Time.deltaTime));
        }

        if (Input.GetKey("s"))
        {
            transform.Translate(0, 0, (-velocidade2 * Time.deltaTime));
        }


    }
}
