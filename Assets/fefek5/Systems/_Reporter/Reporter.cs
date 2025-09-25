using System.Collections;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

namespace fefek5.Systems._Reporter
{
    public class Reporter : MonoBehaviour
    {
        [Header("Notion API Info")] public string notionToken = "empty";
        public string databaseId = "2124c986d99880de90dad5a2c3c29c0c";

        [Button]
        public void SendTaskToNotion(string taskTitle)
        {
            StartCoroutine(SendToNotionCoroutine(taskTitle));
        }

        private IEnumerator SendToNotionCoroutine(string taskTitle)
        {
            const string url = "https://api.notion.com/v1/pages";

            // Replace "Name" with your actual title property key if different
            var jsonBody = $@"
            {{
                ""parent"": {{ ""database_id"": ""{databaseId}"" }},
                ""properties"": {{
                    ""Task name"": {{
                        ""title"": [
                            {{
                                ""text"": {{ ""content"": ""{taskTitle}"" }}
                            }}
                        ]
                    }}
                }}
            }}";

            var request = new UnityWebRequest(url, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Authorization", $"Bearer {notionToken}");
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Notion-Version", "2022-06-28");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Task sent to Notion!");
            }
            else
            {
                Debug.LogError("❌ Failed to send task: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
        
        
        
        [Button]
        public void GetDatabaseProperties()
        {
            StartCoroutine(GetDatabasePropertiesCoroutine());
        }
        
        IEnumerator GetDatabasePropertiesCoroutine()
        {
            var url = $"https://api.notion.com/v1/databases/{databaseId}";
    
            var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", $"Bearer {notionToken}");
            request.SetRequestHeader("Notion-Version", "2022-06-28");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Database schema: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed to get database: " + request.error);
            }
        }
    }
}
