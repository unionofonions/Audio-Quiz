using UnityEngine;

namespace AudioQuiz
{
    public class AppHelper : MonoBehaviour
    {
        public void PauseGame()
        {
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
        }

        public void ExitGame()
        {
            if (Application.isPlaying)
            {
                Application.Quit();
            }
        }
    }
}