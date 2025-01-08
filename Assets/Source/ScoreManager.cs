using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace AudioQuiz
{
    public class ScoreManager : MonoBehaviour
    {
        private const string FileName = @"Data\Scores.json";

        [SerializeField]
        private NameScoreList _list;

        [SerializeField]
        private TextMeshProUGUI[] _guis;

        private void Awake()
        {
            _list = GetList();
        }

        public void AddOrUpdate(string name, int score)
        {
            var existing = _list.Values.FirstOrDefault(p => p.Name == name);
            if (existing != null)
            {
                existing.Score = score;
            }
            else
            {
                _list.Values.Add(new NameScorePair(name, score));
            }

            UpdateDatabase();
        }

        public void UpdateGuis()
        {
            var pairs = GetList().Values.OrderByDescending(p => p.Score).ToArray();
            for (int i = 0; i < Math.Min(pairs.Length, _guis.Length); i++)
            {
                _guis[i].text = $"{pairs[i].Name}: {pairs[i].Score}";
            }
        }

        private void UpdateDatabase()
        {
            string json = JsonUtility.ToJson(_list, true);
            string path = Path.Combine(Application.dataPath, FileName);
            File.WriteAllText(path, json);
            Debug.Log($"Data saved to {path}");
        }

        private NameScoreList GetList()
        {
            string path = Path.Combine(Application.dataPath, FileName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var list = JsonUtility.FromJson<NameScoreList>(json);
                if (list != null)
                {
                    return list;
                }
            }

            return new NameScoreList(new List<NameScorePair>());
        }
    }

    [Serializable]
    public class NameScoreList
    {
        public List<NameScorePair> Values;

        public NameScoreList(List<NameScorePair> values)
        {
            Values = values;
        }
    }

    [Serializable]
    public class NameScorePair
    {
        public string Name;
        public int Score;

        public NameScorePair(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}