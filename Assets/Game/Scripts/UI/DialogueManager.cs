using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Utils.Singleton;
using TMPro;

public class DialogueManager : Singleton<DialogueManager>
{
    [Header("LANGUAGES")]
    private const int en = 0;
    private const int pt = 1;

    private int currentLanguage;

    [Header("DIALOGUE")]
    [SerializeField] private DialogueDetails[] DialogueDetailsArray;
    [SerializeField] private DialogueSpeaker[] DialogueSpeakerArray;

    private int numeroFala = 0;
    private float tempo = 0.0f;
    private float tempoLetras = 0.0f;
    private int letra = 1;
    private bool falasRodando;

    [Header("UI")]
    [SerializeField] private GameObject DialoguePanel;
    [SerializeField] private TextMeshProUGUI falaTexto;
    [SerializeField] private TextMeshProUGUI NomeFalante_Text;
    [SerializeField] private Image Falante_Image;

    [Header("DIALOGUE SPECIFICS")]
    [SerializeField] public string nextScene;

    // Objetos específicos
    // NENHUM AINDA

    private ControleFadePreto _controleFadePreto => ControleFadePreto.I;
    //private PausePanel _pausePanel => PausePanel.I;

    private new void Awake()
    {
        numeroFala = 0;
        falaTexto.text = "";
        currentLanguage = (PlayerPrefs.HasKey("Language") ? PlayerPrefs.GetInt("Language") : 1);
        falasRodando = true;
    }

    void Update()
    {
        DialogueControl();
    }

    void DialogueControl()
    {
        if (falasRodando)
        {
            tempo += Time.deltaTime;
        }

        if (numeroFala == DialogueDetailsArray.Length)
        {
            DialogueOver();
        }
        else
        {
            if (tempo >= DialogueDetailsArray[numeroFala].pauseBeforeDialogue)
            {
                ScriptFalas();
            }
            else
            {
                falaTexto.text = "";
                DialoguePanel.SetActive(false);
            }
        }
    }

    void ScriptFalas()
    {

        //ControleDosObjetosEspecificos(falas);

        string speaker = DialogueSpeakerArray[DialogueDetailsArray[numeroFala].speakerID].speakers[currentLanguage];
        if (speaker == null || speaker == "???")
        {
            Falante_Image.enabled = false;
        }
        else
        {
            Falante_Image.enabled = true;
            Falante_Image.sprite = DialogueSpeakerArray[DialogueDetailsArray[numeroFala].speakerID].speakerSprite;
        }

        string line = DialogueDetailsArray[numeroFala].dialogue[currentLanguage];

        NomeFalante_Text.text = speaker;

        LettersOneByOne(line);

        MouseClick(line);

    }

    void MouseClick(string dialogue)
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if (letra != dialogue.Length + 1)
            {
                falaTexto.text = dialogue;
                letra = dialogue.Length + 1;
            }
            else
            {
                if (tempo > 0.4f)
                {
                    if (numeroFala != DialogueDetailsArray.Length)
                    {
                        tempoLetras = 0.0f;
                        letra = 1;
                        tempo = 0.0f;
                        numeroFala++;
                    }
                }
            }
        }
    }

    void LettersOneByOne(string dialogue)
    {
        tempoLetras += Time.deltaTime;

        if (tempoLetras > 0.05 * letra && letra != dialogue.Length + 1)
        {
            falaTexto.text = dialogue.Substring(0, letra);
            letra++;
        }
    }

    

    void DialogueOver()
    {
        _controleFadePreto.FadeOutScene(nextScene);
    }


    #region Possibly Useful Extra Functions
    //private void OlharParaAlgo()
    //{
    //    if (ondeOlhar != null)
    //    {
    //        if (ondeOlhar.transform.position.x > Player.transform.position.x)
    //        {
    //            Player.transform.localScale = new Vector3(-1, 1, 1);
    //        }
    //        else
    //        {
    //            Player.transform.localScale = new Vector3(1, 1, 1);
    //        }
    //    }
    //}

    //void ControleDosObjetosEspecificos(List<string> falas)
    //{
    //    if (falas[numeroFala] == "EI!" || falas[numeroFala] == "HEY!")
    //    {
    //        LigarObjetosEspecificos("Dragão");
    //    }
    //    else if (falas[numeroFala] == "Você adquiriu uma escama da Senhora Dragão." || falas[numeroFala] == "You have acquired a scale from Ms. Dragon.")
    //    {
    //        LigarObjetosEspecificos("Escama");
    //    }
    //    else if (falas[numeroFala] == "Tá vendo essa porta atrás de mim? Ela sempre levará você ao caminho que precisará seguir." || falas[numeroFala] == "Ya see that door behind me? It will always take you to the path you’ll need to follow.")
    //    {
    //        LigarObjetosEspecificos("Porta");
    //        Mago.transform.localScale = new Vector3(-1, 1, 1);
    //    }
    //    else if (falas[numeroFala] == "VOCÊ!" || falas[numeroFala] == "YOU!")
    //    {
    //        Mago.transform.localScale = new Vector3(1, 1, 1);
    //    }
    //    else if (falas[numeroFala] == "Agora vá!" || falas[numeroFala] == "Now go!")
    //    {
    //        Mago.transform.localScale = new Vector3(1, 1, 1);
    //    }
    //    else if (falas[numeroFala] == "Enfim! É só seguir pela mesma porta que você foi anteriormente." || falas[numeroFala] == "Anyways! Just go through the same door from before." || falas[numeroFala] == "Fique atento." || falas[numeroFala] == "Be aware.")
    //    {
    //        LigarObjetosEspecificos("PortaPassada");
    //        LigarObjetosEspecificos("Porta");
    //    }
    //    else if (falas[numeroFala] == "Vê se não demora!" || falas[numeroFala] == "Try not to take long!" || falas[numeroFala] == "Encontre o Grande Pé de Feijão." || falas[numeroFala] == "Find the Big Beanstalk.")
    //    {
    //        LigarObjetosEspecificos("Porta");
    //    }

    //}

    //void LigarObjetosEspecificos(string objeto, bool state)
    //{
    //    if (objeto == "Dragão")
    //    {
    //        DragonEyes.SetActive(true);
    //    }

    //    if (objeto == "Escama")
    //    {
    //        Escama.SetActive(false);
    //    }
    //}

    #endregion

    #region Extra Classes
    [System.Serializable]
    public class DialogueDetails
    {
        public string[] dialogue;
        public int speakerID;
        public float pauseBeforeDialogue;
    }
    [System.Serializable]
    public class DialogueSpeaker
    {
        public string[] speakers;
        public Sprite speakerSprite;
    }
    #endregion
}
