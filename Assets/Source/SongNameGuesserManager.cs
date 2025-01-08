using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace AudioQuiz
{
    public class SongNameGuesserManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField]
        private AudioClip[] _clips;

        [SerializeField]
        [Min(5f)]
        private float _songDuration;

        [SerializeField]
        [Min(1)]
        private int _questionCount;

        [SerializeField]
        private ScoreManager _scoreManager;

        [SerializeField]
        private UnityEvent _onEnd;

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

        [SerializeField]
        private TextMeshProUGUI _endGameScoreGui;

        private AudioSource _audioSource;
        private AudioClip _currentClip;

        private float _timeLeft;
        private int _currentQuestion;
        private int _correctAnswerCount;
        private int _wrongAnswerCount;
        private int _timeoutCount;

        private string _currentPlayer;

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

        public void SetCurrentPlayer(string player)
        {
            _currentPlayer = player;
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
            _correctAnswerCount++;
            OnAnswer();
        }

        private void OnIncorrectAnswer()
        {
            _wrongAnswerCount++;
            OnAnswer();
        }

        private void OnTimeout()
        {
            _timeoutCount++;
            OnAnswer();
        }

        private void OnAnswer()
        {
            if (++_currentQuestion < _questionCount)
            {
                NextQuestion();
            }
            else
            {
                OnEndGame();
            }
        }

        private void OnEndGame()
        {
            int score = _correctAnswerCount;
            _scoreManager.AddOrUpdate(_currentPlayer, score);
            _onEnd.Invoke();
            StopCurrentClip();
            _endGameScoreGui.text = $"Your Score: {score}";
            enabled = false;
        }

        private void UpdateStopwatch()
        {
            _stopwatchFill.fillAmount = _timeLeft / _songDuration;
            _stopwatchGui.text = ((int)_timeLeft).ToString();
        }

        private void ResetScore()
        {
            _currentQuestion = 0;
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