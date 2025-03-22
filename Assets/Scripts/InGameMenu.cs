using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{

    [SerializeField] private AudioSource audio;

    [SerializeField] private AudioClip[] sound;

    [SerializeField] private GameObject defaultState;
    [SerializeField] private GameObject openState;
    [SerializeField] private GameObject quizState;
    [SerializeField] private GameObject wonState;
    [SerializeField] private GameObject lostState;

    [SerializeField] private GameObject[] stars;
    [SerializeField] private GameObject[] emptyStars;

    [SerializeField] private AudioClip[] lostSounds;
    [SerializeField] private AudioClip[] winSounds;
    [SerializeField] private AudioClip scoreCountLoop;


    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TMP_InputField answerInput;
    [SerializeField] private Button submitButton;

    private Dictionary<int, string[]> levelQuestions = new Dictionary<int, string[]>()    
    {
        { 1, new string[] { "5 + 3 = ?", "9 - 4 = ?", "2 × 5 = ?", "8 + 6 = ?", "12 - 7 = ?", "3 × 3 = ?", "15 - 9 = ?", "4 × 2 = ?", "7 + 9 = ?", "6 × 2 = ?" } },
        { 2, new string[] { "20 ÷ 5 = ?", "Số lớn nhất trong các số: 128, 182, 218?", "Lan có 25 viên kẹo, Minh có 18 viên kẹo. Cả hai có bao nhiêu viên kẹo?", "Một hộp bút có 40 cây, mỗi học sinh lấy 5 cây. Có bao nhiêu học sinh", "Số nào nhỏ nhất trong các số: 450, 540, 504?", "Một con gà có 2 chân, 6 con gà có bao nhiêu chân?", "24 ÷ 4 = ?", "Nếu hôm nay là thứ Tư, 8 ngày nữa là thứ mấy?", "Gấp đôi số 14 thì được bao nhiêu?", "Một trang sách có 10 dòng, mỗi dòng có 7 chữ. Có bao nhiêu chữ trên trang đó?" } },
        { 3, new string[] { "Nếu hôm nay là thứ Ba, 15 ngày nữa là thứ mấy?", "Một số gấp đôi số 18 rồi trừ 6 thì bằng bao nhiêu? ", "Một cửa hàng có 4 kệ sách, mỗi kệ có 8 cuốn sách. Họ bán đi 10 cuốn, còn lại bao nhiêu cuốn?", "Mai có số kẹo gấp 3 lần số kẹo của An. Nếu An có 9 viên kẹo, Mai có bao nhiêu viên?", "Tìm số tiếp theo trong dãy: 2, 4, 8, 16, __?", "Tổng của hai số là 50, số thứ nhất là 28. Số thứ hai là bao nhiêu?", "Một hình vuông có chu vi là 36 cm. Mỗi cạnh dài bao nhiêu?", "Có 30 học sinh xếp thành 5 hàng bằng nhau. Mỗi hàng có bao nhiêu học sinh?", "Một tấm vải dài 90 cm được cắt thành 5 phần bằng nhau. Mỗi phần dài bao nhiêu?", "Một hình chữ nhật có chiều dài 12 cm, chiều rộng 6 cm. Diện tích của nó là bao nhiêu?" } }
    };

    private Dictionary<int, string[]> levelAnswers = new Dictionary<int, string[]>()
    {
        { 1, new string[] { "8", "5", "10", "14", "5", "9", "6", "8", "16", "12" } },
        { 2, new string[] { "4", "218", "43", "8", "450", "12", "6", "thứ năm", "28", "70" } },
        { 3, new string[] { "Thứ Ba", "30", "22", "27", "32", "22", "9 cm", "6", "18 cm", "72 cm vuông" } }
    };
    private int currentQuestionIndex = 0;


    private List<GameObject> Birds;



    private bool onWinScreen = false;


    private void Start()
    {
        submitButton.onClick.AddListener(CheckAnswer);
    }

    private void Awake()
    {
        Birds = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bird"));
    }

    private void FixedUpdate()
    {
        if (GameManager.CurrentGameState == GameState.Won && onWinScreen == false && GameManager.BricksBirdsPigsStoppedMoving() && new List<GameObject>(GameObject.FindGameObjectsWithTag("Bird")).Count == GameManager.birdsNumber)
        {
            int rand = Random.Range(0, 2);
            if (!audio.isPlaying)
            {
                AudioPlayer.audio.PlayOneShot(winSounds[rand]);
            }
            GameObject.Find("ambient").GetComponent<AudioSource>().Stop();
            StartCoroutine(winDelay(rand));
        }
        else if(GameManager.CurrentGameState == GameState.Lost && onWinScreen == false && GameManager.BricksBirdsPigsStoppedMoving() && new List<GameObject>(GameObject.FindGameObjectsWithTag("Bird")).Count == GameManager.birdsNumber)
        {
            GameObject.Find("ambient").GetComponent<AudioSource>().Stop();
            StartCoroutine(lostDelay());
        }
    }

    private void changeState()
    {
        menuSound(1);
        if (defaultState.active == true)
        {
            defaultState.SetActive(false);
            openState.SetActive(true);
        }
        else if(openState.active == true)
        {
            openState.SetActive(false);
            defaultState.SetActive(true);
        }
    }
    private void menuSound(int number)
    {
        audio.PlayOneShot(sound[number]);
    }

    private void reloadLevel()
    {
        menuSound(1);
        Application.LoadLevel(Application.loadedLevel);
    }
    private void loadMenu()
    {
        menuSound(1);
        SceneManager.LoadScene(0);
    }

    private void nextLevel()
    {
        menuSound(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }





    // enums 


    private IEnumerator lostDelay()
    {
        yield return new WaitForSeconds(2f);

        if (GameManager.CurrentGameState == GameState.Lost && onWinScreen == false && GameManager.BricksBirdsPigsStoppedMoving() && new List<GameObject>(GameObject.FindGameObjectsWithTag("Bird")).Count == GameManager.birdsNumber)
        {
            AudioPlayer.audio.PlayOneShot(lostSounds[Random.Range(0, 2)]);


            onWinScreen = true;

            defaultState.SetActive(false);
            openState.SetActive(false);
            lostState.SetActive(true);
        }
    }

public void CheckAnswer()
{
    int currentLevel = SceneManager.GetActiveScene().buildIndex;
    
    if (answerInput.text.Trim().ToLower() == levelAnswers[currentLevel][currentQuestionIndex].ToLower())
    {
        currentQuestionIndex++;
        if (currentQuestionIndex < levelQuestions[currentLevel].Length)
        {
            LoadQuestion();
        }
        else
        {
            quizState.SetActive(false);
            wonState.SetActive(true);
        }
    }
    else
    {
        answerInput.text = ""; // Xóa ô nhập nếu sai
    }
}
private void LoadQuestion()
{
    int currentLevel = SceneManager.GetActiveScene().buildIndex; // Lấy level hiện tại
    if (levelQuestions.ContainsKey(currentLevel))
    {
        questionText.text = levelQuestions[currentLevel][currentQuestionIndex];
    }
    answerInput.text = "";
}
private IEnumerator winDelay(int clipIndex)
{
    yield return new WaitForSeconds(winSounds[clipIndex].length);
    if (GameManager.CurrentGameState == GameState.Won && onWinScreen == false)
    {
        onWinScreen = true;

        defaultState.SetActive(false);
        openState.SetActive(false);
        
        quizState.SetActive(true);
        LoadQuestion();
    }
}


    private IEnumerator starDelay(int num)
    {
        int loopCount = 0;

        int starIndex = 0;

        yield return new WaitForSeconds(1f);
        while (num > loopCount)
        {
            stars[starIndex].SetActive(true);
            emptyStars[starIndex].SetActive(false);

            loopCount++;
            starIndex++;

            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator CountUpToTarget(TextMeshProUGUI label, int targetVal, float duration, float delay = 0f, string prefix = "")
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        int current = 0;
        while (current < targetVal)
        {
            if (AudioPlayer.audio.clip != scoreCountLoop && !AudioPlayer.audio.isPlaying)
            {
                AudioPlayer.audio.PlayOneShot(scoreCountLoop);
            }

            current += (int)(targetVal / (duration / Time.deltaTime));
            current = Mathf.Clamp(current, 0, targetVal);
            label.text = prefix + current;
            yield return null;
        }
    }

}
