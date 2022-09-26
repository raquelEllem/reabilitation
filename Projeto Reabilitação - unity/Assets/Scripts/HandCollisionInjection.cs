using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using System;
using System.Runtime.InteropServices;
using System.IO;


public class HandCollisionInjection : MonoBehaviour
{

    private static bool GoButton = false, ExitButton = false;

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
            if (temp != null && collider.gameObject.name == "Go")
            {
                // rightControllerHaptics.Vibrate(VibrationForce.Light);
                GoButton = true;
                SceneManager.LoadScene("scenes/Fim");
            }
            if (temp != null && collider.gameObject.name == "Exit")
            {
                // rightControllerHaptics.Vibrate(VibrationForce.Medium);
                ExitButton = true;
                SceneManager.LoadScene("scenes/Menu2D");
            }
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


}