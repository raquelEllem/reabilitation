using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using System;
using System.Runtime.InteropServices;
using System.IO;


public class CollisionColorHand : MonoBehaviour
{
    /*Ler o nome do usuário atual*/
    public static string path;
    private string[] userName;
    private string[] users;
    private string[] performance;

    //public GameObject Red, Green, Pink, Orange, Blue;
    private bool vermAlto = false, azulAlto = false, verdeAlto = false, amareloAlto = false, rosaAlto = false;
    private bool vermBaixo = false, azulBaixo = false, verdeBaixo = false, amareloBaixo = false, rosaBaixo = false;
    private int Coll1 = 0, Coll2 = 0;
    private int[] conta;

    public GameObject linhaVermelha, linhaVerde, linhaAzul, linhaAmarela, linhaRosa;


    public Text msg, msgCongrac, nameU;
   

   // [SerializeField]
   // OculusHaptics rightControllerHaptics; //vibra rift touch 

    //enumerador de estados, para manter os estados possíveis para nossa mão
    public enum State
    {
        EMPTY,
        TOUCHING,
        HOLDING
    };

    /* significa qual mão o controlador representa. 
     * Alterar isso para cada mão na janela Unity Inspector */
    public OVRInput.Controller Controller = OVRInput.Controller.LTouch;

    /* variável de estado de mão para ajustar internamente */
    public State mHandState = State.EMPTY;

    /* usado para determinar onde os objetos que pegamos devem ser anexados. 
     * Por padrão, vamos anexá-lo à própria mão, embora você possa alterar 
     * isso para ser outro objeto também em seu aplicativo. */
    public Rigidbody AttachPoint = null;

    /* permite que você pegue o objeto e mantenha-o na mão 
     * com base em onde o pegou */
    public bool IgnoreContactPoint = false;

    /* */
    private Rigidbody mHeldObject;

    /* junta temporária para mover o objeto */
    private FixedJoint mTempJoint;

    /* Após a inicialização das mãos, obteremos o componente RigidBody dentro dele. 
     * Isso será usado como um ponto de anexação para nossas mãos quando pegarmos objetos,
     * se não tivermos especificado um ponto de anexação no Inspetor */
    void Start()
    {
        if (AttachPoint == null)
        {
            AttachPoint = GetComponent<Rigidbody>();
        }

        conta = new int[5];
        msg.text = "Ligue as cores corretamente";

        users = new string[2];
        userName = new string[2];
        performance = new string[5];

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
    }

    /* manipula quando uma mão colide com um objeto. Ele verifica 
     * se já não temos algo nessa mão e, em seguida, garante que o 
     * objeto esteja na camada possível de segurar e tenha um RigidBody anexado a ele.
     * Em seguida, armazenamos em nosso objeto retido e mudamos o estado da mão para tocar. */
    void OnTriggerEnter(Collider collider)
    {

        if (mHandState == State.EMPTY)
        {
            GameObject temp = collider.gameObject;
            if (temp != null && temp.layer == LayerMask.NameToLayer("grabbable") && temp.GetComponent<Rigidbody>() != null)
            {
                mHeldObject = temp.GetComponent<Rigidbody>();
                mHandState = State.TOUCHING;

            }
        }

        //verifica se o usário tocou nos botões Go e Exit
        if (mHandState == State.TOUCHING)
        {
            GameObject temp = collider.gameObject;
       
            /* Identifica o objeto que colidiu com o cursor */
            if (temp != null && collider.gameObject.name == "AmareloAlto")
            {
               // rightControllerHaptics.Vibrate(VibrationForce.Medium);
                amareloAlto = true;
                Coll1 = 1;
            }
            if (temp != null && collider.gameObject.name == "RosaAlto")
            {
                rosaAlto = true;
                Coll1 = 2;
            }
            if (temp != null && collider.gameObject.name == "VerdeAlto")
            {
                verdeAlto = true;
                Coll1 = 3;
            }
            if (temp != null && collider.gameObject.name == "VermelhoAlto")
            {
                vermAlto = true;
                Coll1 = 4;
            }
            if (temp != null && collider.gameObject.name == "AzulAlto")
            {
                azulAlto = true;
                Coll1 = 5;
            }
            if (temp != null && collider.gameObject.name == "AmareloBaixo")
            {
                amareloBaixo = true;
                Coll2 = 1;
            }
            if (temp != null && collider.gameObject.name == "RosaBaixo")
            {
                rosaBaixo = true;
                Coll2 = 2;
            }
            if (temp != null && collider.gameObject.name == "VerdeBaixo")
            {
                verdeBaixo = true;
                Coll2 = 3;
            }
            if (temp != null && collider.gameObject.name == "VermelhoBaixo")
            {
                vermBaixo = true;
                Coll2 = 4;
            }
            if (temp != null && collider.gameObject.name == "AzulBaixo")
            {
                azulBaixo = true;
                Coll2 = 5;
            }

            if (temp != null && collider.gameObject.name == "Go")
            {
                Debug.Log("Próxima Fase");
                SceneManager.LoadScene("scenes/Key");
            }

            if (temp != null && collider.gameObject.name == "Reset")
            {
                Debug.Log("Reset");
                ResetAction();
            }

            if (temp != null && collider.gameObject.name == "Exit")
            {
                Debug.Log("Finalizar");
                SceneManager.LoadScene("scenes/Menu2D");
            }

            ProcessaInformacao();

        }

    }

    /* Faz o oposto do OnTriggerEnter. Em seguida, definimos 
     * o objeto retido como nulo e definimos o estado da mão como vazia */
    void OnTriggerExit(Collider collider)
    {
        if (mHandState != State.HOLDING)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("grabbable"))
            {
                mHeldObject = null;
                mHandState = State.EMPTY;
            }
        }
    }

    // verifica colisão dos botões 
    //void OnCollisionEnter(Collision info)
    //{
    //    /* Identifica se a mão do usuário colidir com os botões Go ou Exit */
    //    if (info.gameObject.name == "Go")
    //    {
    //        //Debug.Log ("Proxima Fase ");
    //        GoButton = true;
    //        SceneManager.LoadScene("scenes/PainelCores");
    //    }
    //    else if (info.gameObject.name == "Exit")
    //    {
    //        ExitButton = true;
    //        SceneManager.LoadScene("scenes/Menu2D");
    //    }
    //}

    /* É onde realmente lidamos com pegar e soltar objetos */
    void Update()
    {
        switch (mHandState)
        {

            /*  Se não houver uma junta temporária e o jogador estiver pressionando o 
             * gatilho com pressão suficiente (> = 0,5f neste caso), ajustamos a velocidade 
             * do objeto segurado para zero e criamos uma junta temporária anexada a ele. 
             * Em seguida, conectamos essa junção ao nosso AttachPoint (por padrão, a própria mão)
             * e definimos o estado da mão como HOLDING */
            case State.TOUCHING:
                if (mTempJoint == null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller) >= 0.5f)
                {
                    mHeldObject.velocity = Vector3.zero;
                    mTempJoint = mHeldObject.gameObject.AddComponent<FixedJoint>();
                    mTempJoint.connectedBody = AttachPoint;
                    mHandState = State.HOLDING;
                }
                break;

            /* Verificamos que temos uma junção temporária (ou seja, que não é nula) 
             * e que o jogador está liberando o suficiente do trigger (neste caso, <0.5f).
             * Nesse caso, destruímos imediatamente a junta temporária e a definimos como nula, 
             * significando que ela não está mais em uso. Em seguida, lançamos o objeto 
             * usando um método throw (descrito mais abaixo) e definimos o estado da mão como EMPTY. */
            case State.HOLDING:
                if (mTempJoint != null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, Controller) < 0.5f)
                {
                    // Object.DestroyImmediate(mTempJoint);
                    UnityEngine.Object.DestroyImmediate(mTempJoint);
                    mTempJoint = null;
                    throwObject();
                    mHandState = State.EMPTY;
                }
                break;
        }
    }

    /* atualmente ele tem um bug onde os valores para a velocidade 
     * angular são fixados em [0, 2 * Pi]. O que isto significa é 
     * que quando você joga um objeto, a direção rotacional pode 
     * às vezes ser incorreta com base em como ele é lançado.
     * Por enquanto, o que fazemos é começar a definir a 
     * velocidade do objeto segurado para a velocidade do controlador. 
     * Em seguida, definimos a velocidade angular para a velocidade 
     * angular do controlador também. Finalmente, definimos a velocidade 
     * angular máxima para a magnitude da velocidade angular, 
     * garantindo que a rotação não enlouqueça*/
    private void throwObject()
    {
        mHeldObject.velocity = OVRInput.GetLocalControllerVelocity(Controller);
        mHeldObject.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller) * Mathf.Deg2Rad;
        mHeldObject.maxAngularVelocity = mHeldObject.angularVelocity.magnitude;
    }

    void ResetFunction()
    {
        Coll1 = 0;
        Coll2 = 0;
        azulBaixo = false;
        azulAlto = false;
        amareloBaixo = false;
        amareloAlto = false;
        vermBaixo = false;
        vermAlto = false;
        verdeBaixo = false;
        verdeAlto = false;
        rosaAlto = false;
        rosaBaixo = false;


        //Reset the writing on the board
        //if(ConverterClass.ConvertIntPtrToByteToString( PluginImport.GetTouchedObjectName()) == "reset") // GetTouchedObjectName - To be deprecated
        //if (ConverterClass.ConvertIntPtrToByteToString (PluginImport.GetTouchedObjName (1)) == "Reset") {
        //	myWritingScript.cleanBoard ();

        //}
    }

    void ResetAction()
    {
        Coll1 = 0;
        Coll2 = 0;
        azulBaixo = false;
        azulAlto = false;
        amareloBaixo = false;
        amareloAlto = false;
        vermBaixo = false;
        vermAlto = false;
        verdeBaixo = false;
        verdeAlto = false;
        rosaAlto = false;
        rosaBaixo = false;

        msgCongrac.text = "";

        // retorna a posição Z inical
        Vector3 newPositionAmarelo = new Vector3(-1.55f, 0.11f, 0.22f);
        linhaAmarela.gameObject.transform.localPosition = newPositionAmarelo;

        Vector3 newPositionRosa = new Vector3(0.46f, 0.11f, 0.22f);
        linhaRosa.gameObject.transform.localPosition = newPositionRosa;

        Vector3 newPositionVerde = new Vector3(-2.85f, 0.11f, 0.22f);
        linhaVerde.gameObject.transform.localPosition = newPositionVerde;

        Vector3 newPositionVermelho = new Vector3(-2.95f, 0.11f, 0.22f);
        linhaVermelha.gameObject.transform.localPosition = newPositionVermelho;

        Vector3 newPositionAzul = new Vector3(-1.07f, 0.11f, 0.22f);
        linhaAzul.gameObject.transform.localPosition = newPositionAzul;

    }

    void ProcessaInformacao()
    {

        try
        {
            path = "./Files/color.txt";
            Debug.Log("Arquivo injection aberto com sucesso!");
        }
        catch
        {
            Debug.Log("Erro ao abrir o arquivo!");
        }


        if (amareloAlto && amareloBaixo)
        {
            if (Coll1 == 1 && Coll2 == 1)
            {
                // muda a posição em Z da linha, indicando que o usuário ligou as cores
                Vector3 newPositionAmarelo = new Vector3(-1.55f, 0.11f, 0.05f);
                linhaAmarela.gameObject.transform.localPosition = newPositionAmarelo;

                msgCongrac.text = "Cor Amarela Ok! Parabéns! ";
                performance[0] = "amarelo";
            }
            else
            {
                Coll1 = 0;
                Coll2 = 0;
                ResetFunction();
            }
        }
        else if (rosaBaixo && rosaAlto)
        {
            if (Coll1 == 2 && Coll2 == 2)
            {
                Vector3 newPositionRosa = new Vector3(0.46f, 0.11f, 0.05f);
                linhaRosa.gameObject.transform.localPosition = newPositionRosa;

                msgCongrac.text = "Cor Rosa Ok! Parabéns! ";
                performance[1] = "rosa";
            }
            else
            {
                Coll1 = 0;
                Coll2 = 0;
                ResetFunction();
            }
        }
        else if (verdeBaixo && verdeAlto)
        {
            if (Coll1 == 3 && Coll2 == 3)
            {
                Vector3 newPositionVerde = new Vector3(-2.85f, 0.11f, 0.05f);
                linhaVerde.gameObject.transform.localPosition = newPositionVerde;

                msgCongrac.text = "Cor Verde Ok! Parabéns! ";
                performance[2] = "verde";
            }
            else
            {
                Coll1 = 0;
                Coll2 = 0;
                ResetFunction();
            }
        }
        else if (vermBaixo && vermAlto)
        {
            if (Coll1 == 4 && Coll2 == 4)
            {
                Vector3 newPositionVermelho = new Vector3(-2.95f, 0.11f, 0.05f);
                linhaVermelha.gameObject.transform.localPosition = newPositionVermelho;

                msgCongrac.text = "Cor Vermelho Ok! Parabéns! ";
                performance[3] = "vermelho";
            }
            else
            {
                Coll1 = 0;
                Coll2 = 0;
                ResetFunction();
            }
        }
        else if (azulBaixo && azulAlto)
        {
            if (Coll1 == 5 && Coll2 == 5)
            {
                Vector3 newPositionAzul = new Vector3(-1.07f, 0.11f, 0.05f);
                linhaAzul.gameObject.transform.localPosition = newPositionAzul;

                msgCongrac.text = "Cor Azul Ok! Parabéns! ";
                performance[4] = "azul";
            }
            else
            {
                Coll1 = 0;
                Coll2 = 0;
                ResetFunction();
            }
        }

        File.WriteAllLines(path, performance);


    }

}



