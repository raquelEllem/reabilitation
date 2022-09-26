using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using System;
using System.Runtime.InteropServices;
using System.IO;


public class HandCollisionFase12 : MonoBehaviour
{

    /*Ler o nome do usuário atual*/
    public static string path;
    private string[] userName;
    private string[] users;
    private string[] performance;

    /* Para enviar e receber informações da classe Genérica */
    //public Configuration11 c11;

    public GameObject i0, i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13,
   i14, i15, i16, i17, i18, i19, i20, i21, i22, i23, i24, i25, i26, i27, i28, i29,
   i30, i31, i32, i33, i34, i35, i36, i37, i38, i39, i40, i41, i42, i43, i44, i45,
   i46, i47, i48, i49, i50, i51, i52, i53, i54, i55, i56, i57, i58, i59, i60, i61,
   i62, i63, i64, i65, i66, i67, i68, i69, iN;

    public GameObject Go, Reset, Exit;

    /* Textos */
    public Text msgPercent, msgUnique, msgCongrac, msgTips, nameU;

    private static bool Begin = false, End = false, Trajectory = false, GoButton = false, Wall = false,
                 ResetButton = false, Completou = false, ExitButton = false;

    private bool Marker0 = false, Marker1 = false, Marker2 = false, Marker3 = false, Marker4 = false, Marker5 = false,
        Marker6 = false, Marker7 = false, Marker8 = false, Marker9 = false, Marker10 = false, Marker11 = false,
        Marker12 = false, Marker13 = false, Marker14 = false, Marker15 = false, Marker16 = false, Marker17 = false,
        Marker18 = false, Marker19 = false, Marker20 = false, Marker21 = false, Marker22 = false, Marker23 = false,
        Marker24 = false, Marker25 = false, Marker26 = false, Marker27 = false, Marker28 = false, Marker29 = false,
        Marker30 = false, Marker31 = false, Marker32 = false, Marker33 = false, Marker34 = false, Marker35 = false,
        Marker36 = false, Marker37 = false, Marker38 = false, Marker39 = false, Marker40 = false, Marker41 = false,
        Marker42 = false, Marker43 = false, Marker44 = false, Marker45 = false, Marker46 = false, Marker47 = false,
        Marker48 = false, Marker49 = false, Marker50 = false, Marker51 = false, Marker52 = false, Marker53 = false,
        Marker54 = false, Marker55 = false, Marker56 = false, Marker57 = false, Marker58 = false, Marker59 = false,
        Marker60 = false, Marker61 = false, Marker62 = false, Marker63 = false, Marker64 = false, Marker65 = false,
        Marker66 = false, Marker67 = false, Marker68 = false, Marker69 = false, MarkerN = false;


    /* Identificar quantos marcadores foram utilizados 
	 * para desenvolver a linha do trajeto percorrido 
	 * pelo usuário */
    private float totalDeMarcadoresLinhas = 71;

    /* Informação enviada à classe Genérica para calcular o 
	 * percentual que é exibido em tempo real para o 
	 * usuário */
    private float marcados = 0, percentualMarcado = 0;

    /* Vetores que armazenam os marcadores
	 * percorridos para saber quais já tiveram a colisão */
    private int[] marcadoLinhas = new int[71];


    public static bool newReset, beginOK = false; //info recebida da classe generica para reiniciar 
    private bool fase = false;
    //static public int mudaFase = 1, qualFase = 0, defineConfiguracoes = 0; //Controlar a mudança de fase

    /*  Booleans Variables */
    //public bool hit = false;

    private int noColision = 0;

    private bool deviation = false;

    //public GameObject Go, Exit;



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

        //Garante que os vetores de marcação inicializem com 0
        for (int i = 0; i < marcadoLinhas.Length; i++)
        {
            marcadoLinhas[i] = 0;
        }

    }


    public void VerificaBegin(bool b)
    {

        beginOK = b;

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
            if (temp != null && collider.gameObject.name == "Marker0")
            {
                //Debug.Log ("Begin ");
                Begin = true;
                msgTips.text = "";
            }
            if (temp != null && collider.gameObject.name == "MarkerN")
            {
                //Debug.Log ("End ");
                End = true;
            }
            if (temp != null && collider.gameObject.name == "Wall")
            {
                //Debug.Log ("Wall ");
                Wall = true;
            }
            if (temp != null && collider.gameObject.name == "Go")
            {
                //Debug.Log ("Proxima Fase ");
                GoButton = true;
            }
            if (temp != null && collider.gameObject.name == "Reset")
            {
                //Debug.Log ("Reset");
               // ResetButton = true;
                //ResetAction(true);
            }
            if (temp != null && collider.gameObject.name == "Exit")
            {
                //ExitButton = true;
                //SceneManager.LoadScene("scenes/Menu2D");
            }
            if (collider.gameObject.name == "Marker1" || collider.gameObject.name == "Marker2" || collider.gameObject.name == "Marker3" ||
                 collider.gameObject.name == "Marker4" || collider.gameObject.name == "Marker5" || collider.gameObject.name == "Marker6" ||
                 collider.gameObject.name == "Marker7" || collider.gameObject.name == "Marker8" || collider.gameObject.name == "Marker9" ||
                 collider.gameObject.name == "Marker10" || collider.gameObject.name == "Marker11" || collider.gameObject.name == "Marker12" ||
                 collider.gameObject.name == "Marker13" || collider.gameObject.name == "Marker14" || collider.gameObject.name == "Marker15" ||
                 collider.gameObject.name == "Marker16" || collider.gameObject.name == "Marker17" || collider.gameObject.name == "Marker18" ||
                 collider.gameObject.name == "Marker19" || collider.gameObject.name == "Marker20" || collider.gameObject.name == "Marker21" ||
                 collider.gameObject.name == "Marker22" || collider.gameObject.name == "Marker23" || collider.gameObject.name == "Marker24" ||
                 collider.gameObject.name == "Marker25" || collider.gameObject.name == "Marker26" || collider.gameObject.name == "Marker27" ||
                 collider.gameObject.name == "Marker28" || collider.gameObject.name == "Marker29" || collider.gameObject.name == "Marker30" ||
                 collider.gameObject.name == "Marker31" || collider.gameObject.name == "Marker32" || collider.gameObject.name == "Marker33" ||
                 collider.gameObject.name == "Marker34" || collider.gameObject.name == "Marker35" || collider.gameObject.name == "Marker36" ||
                 collider.gameObject.name == "Marker37" || collider.gameObject.name == "Marker38" || collider.gameObject.name == "Marker39" ||
                 collider.gameObject.name == "Marker40" || collider.gameObject.name == "Marker41" || collider.gameObject.name == "Marker42" ||
                 collider.gameObject.name == "Marker43" || collider.gameObject.name == "Marker44" || collider.gameObject.name == "Marker45" ||
                 collider.gameObject.name == "Marker46" || collider.gameObject.name == "Marker47" || collider.gameObject.name == "Marker48" ||
                 collider.gameObject.name == "Marker49" || collider.gameObject.name == "Marker50" || collider.gameObject.name == "Marker51" ||
                 collider.gameObject.name == "Marker52" || collider.gameObject.name == "Marker53" || collider.gameObject.name == "Marker54" ||
                 collider.gameObject.name == "Marker55" || collider.gameObject.name == "Marker56" || collider.gameObject.name == "Marker57" ||
                 collider.gameObject.name == "Marker58" || collider.gameObject.name == "Marker59" || collider.gameObject.name == "Marker60" ||
                 collider.gameObject.name == "Marker61" || collider.gameObject.name == "Marker62" || collider.gameObject.name == "Marker63" ||
                 collider.gameObject.name == "Marker64" || collider.gameObject.name == "Marker65" || collider.gameObject.name == "Marker66" ||
                 collider.gameObject.name == "Marker67" || collider.gameObject.name == "Marker68" || collider.gameObject.name == "Marker69")
            {
                //Debug.Log ("Trajeto");
                Trajectory = true;

            }
            if (temp != null && collider.gameObject.name == "Marker0")
            {
                Marker0 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker1")
            {
                Marker1 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker2")
            {
                Marker2 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker3")
            {
                Marker3 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker4")
            {
                Marker4 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker5")
            {
                Marker5 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker6")
            {
                Marker6 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker7")
            {
                Marker7 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker8")
            {
                Marker8 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker9")
            {
                Marker9 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker10")
            {
                Marker10 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker11")
            {
                Marker11 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker12")
            {
                Marker12 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker13")
            {
                Marker13 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker14")
            {
                Marker14 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker15")
            {
                Marker15 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker16")
            {
                Marker16 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker17")
            {
                Marker17 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker18")
            {
                Marker18 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker19")
            {
                Marker19 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker20")
            {
                Marker20 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker21")
            {
                Marker21 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker22")
            {
                Marker22 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker23")
            {
                Marker23 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker24")
            {
                Marker24 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker25")
            {
                Marker25 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker26")
            {
                Marker26 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker27")
            {
                Marker27 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker28")
            {
                Marker28 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker29")
            {
                Marker29 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker30")
            {
                Marker30 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker31")
            {
                Marker31 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker32")
            {
                Marker32 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker33")
            {
                Marker33 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker34")
            {
                Marker34 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker35")
            {
                Marker35 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker36")
            {
                Marker36 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker37")
            {
                Marker37 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker38")
            {
                Marker38 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker39")
            {
                Marker39 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker40")
            {
                Marker40 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker41")
            {
                Marker41 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker42")
            {
                Marker42 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker43")
            {
                Marker43 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker44")
            {
                Marker44 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker45")
            {
                Marker45 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker46")
            {
                Marker46 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker47")
            {
                Marker47 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker48")
            {
                Marker48 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker49")
            {
                Marker49 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker50")
            {
                Marker50 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker51")
            {
                Marker51 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker52")
            {
                Marker52 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker53")
            {
                Marker53 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker54")
            {
                Marker54 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker55")
            {
                Marker55 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker56")
            {
                Marker56 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker57")
            {
                Marker57 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker58")
            {
                Marker58 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker59")
            {
                Marker59 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker60")
            {
                Marker60 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker61")
            {
                Marker61 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker62")
            {
                Marker62 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker63")
            {
                Marker63 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker64")
            {
                Marker64 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker65")
            {
                Marker65 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker66")
            {
                Marker66 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker67")
            {
                Marker67 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker68")
            {
                Marker68 = true;
            }
            if (temp != null && collider.gameObject.name == "Marker69")
            {
                Marker69 = true;
            }
            if (temp != null && collider.gameObject.name == "MarkerN")
            {
                MarkerN = true;
            }

            AcaoColisao();
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


    /*
	************************************************************************************
	 Processa as informações de colisão para calcular o percentual de acerto
	 e exibir as mensagens aos usuários  
	************************************************************************************
	*/
    public void ProcessInformation()
    {
        Debug.Log("ProcessInformation");
        percentualMarcado = (marcados / totalDeMarcadoresLinhas) * 100;


        if (Begin)
        {

            beginOK = true;

            msgPercent.text = percentualMarcado.ToString("F0") + " % ";

            if (percentualMarcado < 100)
            {
                msgUnique.text = "Percorra todo o trajeto até o final!";
                Debug.Log("Percorra todo o trajeto!");
                msgTips.text = "";
                msgCongrac.text = "";
                //Wall = false;
            }
            else if (End && percentualMarcado == 100)
            {
                Completou = true;
                msgCongrac.text = "Parabéns!";
                msgUnique.text = "";
                Debug.Log("Parabéns!");
                SavePerformanceInformation();

            }

            if (Trajectory == true && Wall == true)
            {
                Debug.Log("Volte para a trajetoria definida pela linha!");
            }

        }//Fim do if Begin



        if (Completou && GoButton)
        {
            Debug.Log("Chama Muda Fase");
            MudaFase();
        }
        else if (!Completou && GoButton)
        {
            msgCongrac.text = "";
            GoButton = false;
        }//Fim

        if (Trajectory)
        {
            if (!Begin && End)
            {
                msgUnique.text = "Vá para o início da trajetória!";
                Debug.Log(" Vá para o início da trajetoria!");
                msgTips.text = "";
                msgCongrac.text = "";
                End = false;
            }
            else if (!Begin && !End)
            {
                msgUnique.text = "Vá para o início da trajetória!";
                Debug.Log(" Vá para o início da trajetoria!");
                msgTips.text = "";
                msgCongrac.text = "";
            }
        }//Fim do if Trajectory


    }//Fim da função ProcessInformation

    /*************************************************************************************/
    /* Recebe da GenericFunctionClass a informação de resetar 
	 * as variáveis de cálculo de marcados */
    /*************************************************************************************/
    public void ResetAction(bool resetou)
    {

        newReset = resetou;

        //hit = false;
        GoButton = false;
        Begin = false;
        End = false;
        Trajectory = false;
        noColision = 0;
        marcados = 0;
        Wall = false;
        deviation = false;
        percentualMarcado = 0;
        msgUnique.text = "";
        msgCongrac.text = "";
        msgPercent.text = "";


    }


    /*************************************************************************************/
    /* Usado para realizar alguma ação quando a colisão termina */
    /*************************************************************************************/
    void OnCollisionExit(Collision info)
    {

        //a.DesvioDaLinha(true);
        Debug.Log("Sem colisao");
    }

    /*************************************************************************************/
    /* Muda a Fase */
    /*************************************************************************************/
    public void MudaFase()
    {

        ResetAction(true);
        SceneManager.LoadScene("scenes/Fase13"); //Segunda fase das Linhas

    }

    public void SavePerformanceInformation()
    {

        try
        {
            path = "./Files/fase12.txt";
            Debug.Log("Arquivo 2 aberto com sucesso!");
        }
        catch
        {
            Debug.Log("Erro ao abrir o arquivo!");
        }

        if (Completou == true)
        {
            performance[0] = "true";
        }
        else
        {
            performance[0] = "false";
        }

        File.WriteAllLines(path, performance);
    }


    public void AcaoColisao()
    {
        ProcessInformation();

        //zera os marcadores para o caso de o usuário errar ou reiniciar 
        if (newReset)
        {

            marcados = 0;
            VerificaBegin(false);
            for (int i = 0; i < marcadoLinhas.Length; i++)
            {
                marcadoLinhas[i] = 0;
            }

            //Para que volte a identificar colisões após resetar
            newReset = false;

            //Volta a cor dos marcadores para preto
            i0.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i1.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i2.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i3.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i4.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i5.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i6.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i7.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i8.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i9.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i10.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i11.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i12.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i13.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i14.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i15.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i16.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i17.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i18.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i19.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i20.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i21.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i22.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i23.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i24.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i25.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i26.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i27.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i28.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i29.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i30.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i31.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i32.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i33.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i34.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i35.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i36.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i37.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i38.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i39.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i40.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i41.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i42.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i43.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i44.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i45.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i46.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i47.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i48.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i49.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i50.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i51.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i52.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i53.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i54.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i55.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i56.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i57.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i58.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i59.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i60.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i61.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i62.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i63.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i64.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i65.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i66.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i67.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i68.gameObject.GetComponent<Renderer>().material.color = Color.black;
            i69.gameObject.GetComponent<Renderer>().material.color = Color.black;
            iN.gameObject.GetComponent<Renderer>().material.color = Color.black;
        }
        else
        {

            /* Só marca os percorridos e troca a cor quando, de fato, o usuário iniciou a trajetória */
            if (beginOK)
            {

                /* Verifica qual objeto marcador recebeu a colisão e se a colisão já não foi marcada no vetor de colisões  */
                if (Marker0 == true && marcadoLinhas[0] == 0)
                {

                    msgPercent.text = "1%";
                    //Conta a quantidade de objetos marcadores que sofreram a colisão para calcular o %
                    marcados = 1;
                    //Adiciona ao vetor de marcados a posição do objeto que sofreu a colisão
                    marcadoLinhas[0] = 1;
                    //Troca a cor ao colidir
                    i0.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao 						

                }
                else if (Marker1 == true && marcadoLinhas[1] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[1] = 1;
                    i1.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir						

                }
                else if (Marker2 == true && marcadoLinhas[2] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[2] = 1;
                    i2.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir

                }
                else if (Marker3 == true && marcadoLinhas[3] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[3] = 1;
                    i3.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir

                }
                else if (Marker4 == true && marcadoLinhas[4] == 0)
                {


                    marcados = marcados + 1;
                    marcadoLinhas[4] = 1;
                    i4.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir

                }
                else if (Marker5 == true && marcadoLinhas[5] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[5] = 1;
                    i5.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir

                }
                else if (Marker6 == true && marcadoLinhas[6] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[6] = 1;
                    i6.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir

                }
                else if (Marker7 == true && marcadoLinhas[7] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[7] = 1;
                    i7.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker8 == true && marcadoLinhas[8] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[8] = 1;
                    i8.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker9 == true && marcadoLinhas[9] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[9] = 1;
                    i9.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker10 == true && marcadoLinhas[10] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[10] = 1;
                    i10.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker11 == true && marcadoLinhas[11] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[11] = 1;
                    i11.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker12 == true && marcadoLinhas[12] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[12] = 1;
                    i12.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker13 == true && marcadoLinhas[13] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[13] = 1;
                    i13.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker14 == true && marcadoLinhas[14] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[14] = 1;
                    i14.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir

                }
                else if (Marker15 == true && marcadoLinhas[15] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[15] = 1;
                    i15.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker16 == true && marcadoLinhas[16] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[16] = 1;
                    i16.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir
                }
                else if (Marker17 == true && marcadoLinhas[17] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[17] = 1;
                    i17.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir
                }
                else if (Marker18 == true && marcadoLinhas[18] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[18] = 1;
                    i18.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir
                }
                else if (Marker19 == true && marcadoLinhas[19] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[19] = 1;
                    i19.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir
                }
                else if (Marker20 == true && marcadoLinhas[20] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[20] = 1;
                    i20.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir
                }
                else if (Marker21 == true && marcadoLinhas[21] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[21] = 1;
                    i21.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir
                }
                else if (Marker22 == true && marcadoLinhas[22] == 0)
                {

                marcados = marcados + 1;
                marcadoLinhas[22] = 1;
                i22.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker23 == true && marcadoLinhas[23] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[23] = 1;
                    i23.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker24 == true && marcadoLinhas[24] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[24] = 1;
                    i24.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker25 == true && marcadoLinhas[25] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[25] = 1;
                    i25.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker26 == true && marcadoLinhas[26] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[26] = 1;
                    i26.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker27 == true && marcadoLinhas[27] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[27] = 1;
                    i27.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker28 == true && marcadoLinhas[28] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[28] = 1;
                    i28.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker29 == true && marcadoLinhas[29] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[29] = 1;
                    i29.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker30 == true && marcadoLinhas[30] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[30] = 1;
                    i30.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker31 == true && marcadoLinhas[31] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[31] = 1;
                    i31.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker32 == true && marcadoLinhas[32] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[32] = 1;
                    i32.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker33 == true && marcadoLinhas[33] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[33] = 1;
                    i33.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker34 == true && marcadoLinhas[34] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[34] = 1;
                    i34.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker35 == true && marcadoLinhas[35] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[35] = 1;
                    i35.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else if (Marker36 == true && marcadoLinhas[36] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[36] = 1;
                    i36.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker37 == true && marcadoLinhas[37] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[37] = 1;
                    i37.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker38 == true && marcadoLinhas[38] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[38] = 1;
                    i38.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker39 == true && marcadoLinhas[39] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[39] = 1;
                    i39.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker40 == true && marcadoLinhas[40] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[40] = 1;
                    i40.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker41 == true && marcadoLinhas[41] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[41] = 1;
                    i41.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker42 == true && marcadoLinhas[42] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[42] = 1;
                    i42.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker43 == true && marcadoLinhas[43] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[43] = 1;
                    i43.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker44 == true && marcadoLinhas[44] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[44] = 1;
                    i44.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker45 == true && marcadoLinhas[45] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[45] = 1;
                    i45.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker46 == true && marcadoLinhas[46] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[46] = 1;
                    i46.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker47 == true && marcadoLinhas[47] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[47] = 1;
                    i47.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker48 == true && marcadoLinhas[48] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[48] = 1;
                    i48.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker49 == true && marcadoLinhas[49] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[49] = 1;
                    i49.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker50 == true && marcadoLinhas[50] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[50] = 1;
                    i50.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker51 == true && marcadoLinhas[51] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[51] = 1;
                    i51.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker52 == true && marcadoLinhas[52] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[52] = 1;
                    i52.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker53 == true && marcadoLinhas[53] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[53] = 1;
                    i53.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker54 == true && marcadoLinhas[54] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[54] = 1;
                    i54.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker55 == true && marcadoLinhas[55] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[55] = 1;
                    i55.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker56 == true && marcadoLinhas[56] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[56] = 1;
                    i56.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker57 == true && marcadoLinhas[57] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[57] = 1;
                    i57.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker58 == true && marcadoLinhas[58] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[58] = 1;
                    i58.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker59 == true && marcadoLinhas[59] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[59] = 1;
                    i59.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker60 == true && marcadoLinhas[60] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[60] = 1;
                    i60.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker61 == true && marcadoLinhas[61] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[61] = 1;
                    i61.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker62 == true && marcadoLinhas[62] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[62] = 1;
                    i62.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker63 == true && marcadoLinhas[63] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[63] = 1;
                    i63.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker64 == true && marcadoLinhas[64] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[64] = 1;
                    i64.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker65 == true && marcadoLinhas[65] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[65] = 1;
                    i65.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker66 == true && marcadoLinhas[66] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[66] = 1;
                    i66.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker67 == true && marcadoLinhas[67] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[67] = 1;
                    i67.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker68 == true && marcadoLinhas[68] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[68] = 1;
                    i68.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (Marker69 == true && marcadoLinhas[69] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[69] = 1;
                    i69.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
                else if (MarkerN == true && marcadoLinhas[70] == 0)
                {

                    marcados = marcados + 1;
                    marcadoLinhas[70] = 1;
                    iN.gameObject.GetComponent<Renderer>().material.color = Color.gray; //Troca a cor ao colidir


                }
        }

        }

    }

}
