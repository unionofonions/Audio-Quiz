using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AudioQuiz
{
    public class SongNameGuesserManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField]
        private AudioClip[] _clips;

        [SerializeField]
        private float _songDuration;

        [Header("UI")]
        [SerializeField]
        private Button[] _quizOptions;

        [SerializeField]
        private Image _stopwatchFill;

        [SerializeField]
        private TextMeshProUGUI _stopwatchGui;

        [SerializeField]
        private TextMeshProUGUI _correctAnswerCountGui;

        [SerializeField]
        private TextMeshProUGUI _wrongAnswerCountGui;

        [SerializeField]
        private TextMeshProUGUI _timeoutCountGui;

        private AudioSource _audioSource;
        private AudioClip _currentClip;

        private float _timeLeft;
        private int _correctAnswerCount;
        private int _wrongAnswerCount;
        private int _timeoutCount;

        public void StartGame()
        {
            ResetScore();
            NextQuestion();
            enabled = true;
        }

        public void PlayCurrentClip()
        {
            _audioSource.clip = _currentClip;
            _audioSource.Play();
        }

        public void StopCurrentClip()
        {
            _audioSource.Stop();
        }

        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.loop = true;
            enabled = false;
        }

        private void Update()
        {
            _timeLeft -= Time.deltaTime;
            UpdateStopwatch();
            if (_timeLeft <= 0f)
            {
                OnTimeout();
            }
        }

        private void SetQuizOptions()
        {
            ShuffleClips();

            int correctIndex = Random.Range(0, _quizOptions.Length);
            _currentClip = _clips[correctIndex];

            for (int i = 0; i < _quizOptions.Length; i++)
            {
                var option = _quizOptions[i];
                var gui = option.GetComponentInChildren<TextMeshProUGUI>();

                gui.text = _clips[i].name;
                option.onClick.RemoveAllListeners();
                option.onClick.AddListener(i == correctIndex ? OnCorrectAnswer : OnIncorrectAnswer);
            }
        }

        private void NextQuestion()
        {
            SetQuizOptions();
            StopCurrentClip();
            UpdateScoreUI();
            _timeLeft = _songDuration;
        }

        private void OnCorrectAnswer()
        {
            Debug.Log("CORRECT answer.");
            _correctAnswerCount++;
            NextQuestion();
        }

        private void OnIncorrectAnswer()
        {
            Debug.Log("WRONG answer.");
            _wrongAnswerCount++;
            NextQuestion();
        }

        private void OnTimeout()
        {
            Debug.Log("TIMEOUT answer.");
            _timeoutCount++;
            NextQuestion();
        }

        private void UpdateStopwatch()
        {
            _stopwatchFill.fillAmount = _timeLeft / _songDuration;
            _stopwatchGui.text = ((int)_timeLeft).ToString();
        }

        private void ResetScore()
        {
            _correctAnswerCount = 0;
            _wrongAnswerCount = 0;
            _timeoutCount = 0;
        }

        private void UpdateScoreUI()
        {
            _correctAnswerCountGui.text = _correctAnswerCount.ToString();
            _wrongAnswerCountGui.text = _wrongAnswerCount.ToString();
            _timeoutCountGui.text = _timeoutCount.ToString();
        }

        private void ShuffleClips()
        {
            int n = _clips.Length;
            for (int i = 0; i < n - 1; i++)
            {
                int j = Random.Range(i, n);
                if (j != i)
                {
                    (_clips[j], _clips[i]) = (_clips[i], _clips[j]);
                }
            }
        }
    }
}