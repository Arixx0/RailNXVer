using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Data
{
    // NOTE:
    //  This class is used to download Google Sheets data and save it as CSV file.
    //  This class is editor-only.
    public class GoogleSheetsUtility
    {
        #region Constants
        
        private const string GoogleSheetsURLPrefix = "https://docs.google.com/spreadsheets/d/";
        
        private const string CsvExportURL = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&usp=sharing&gid={1}";
        
        #endregion
        
        #region Properties
        
        public string Content { get; private set; }

        public bool IsCompleted { get; private set; } = true;
        
        #endregion
        
        #region Google Sheets Helper Methods

        public static bool TryGetGoogleSheetMeta(string fromURL, out string sheetKey, out string sheetGid)
        {
            if (!fromURL.StartsWith(GoogleSheetsURLPrefix))
            {
                sheetKey = "invalid-url";
                sheetGid = "invalid-url";
                return false;
            }

            var urlElement = fromURL.Split('/');
            sheetKey = urlElement[5];
            // sheetGid = urlElement[6].Split('=')[^1];
            sheetGid = Regex.Match(urlElement[6], @"gid=(\d+)").Groups[1].Value;
            return true;
        }
        
        public static string GetCsvExportURL(string fileKey, string gid)
        {
            return string.Format(CsvExportURL, fileKey, gid);
        }
        
        public async void DownloadFromUrl(string url)
        {
            IsCompleted = false;
            
            using var httpClient = new HttpClient();
            using var httpRequest = new HttpRequestMessage(new HttpMethod("GET"), url);
            var httpResponse = await httpClient.SendAsync(httpRequest);
            Content = await httpResponse.Content.ReadAsStringAsync();

            IsCompleted = true;
        }

        public static void SaveCsvFileToPath(string filePath, string csvContent)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("Given file path is null or empty. Check it again.");
                return;
            }
            
            if (!filePath.EndsWith(".csv"))
            {
                Debug.LogWarning("File path is invalid: ");
                return;
            }
            
            if (Directory.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            // Parse parent directory
            var directoryName = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(directoryName))
            {
                Debug.LogWarning("Was not able to parse directory name. Check and retry.");
                return;
            }
            Directory.CreateDirectory(directoryName);

            // Write content to file
            var fileStream = File.Create(filePath);
            using var outputStream = new StreamWriter(fileStream, Encoding.UTF8);
            outputStream.Write(csvContent);
        }
        
        #endregion
    }
    
    public class GoogleSheetsHelperEditorWindow : EditorWindow
    {
        #region Google Sheets Helper Properties
        
        private readonly GoogleSheetsUtility m_Utility = new();
        
        private string m_GoogleSheetsURL;
        
        private string m_GoogleSheetsKey;
        
        private string m_SheetGid;
        
        private string m_CsvExportURL;
        
        private string m_DatabaseFileName;
        
        private int m_SelectedSaveLocationIndex;
        
        private string m_FinalSaveLocation;

        private bool m_IsGoogleSheetMetadataValid;

        private bool m_IsFileNameValid;

        private bool m_IsFileExistsAtGivenLocation;
        
        #endregion
        
        #region Property Labels
        
        private static readonly GUIContent GoogleSheetsURLLabel = new("Google Sheets URL");
        
        private static readonly GUIContent SheetKeyLabel = new("Google Sheets Key");
        
        private static readonly GUIContent SheetGIDLabel = new("Google Sheets GID");
        
        private static readonly GUIContent CsvExportURLLabel = new("CSV Export URL");
        
        private static readonly GUIContent DatabaseFileNameLabel = new("Database File Name");
        
        private static readonly GUIContent SaveFileLocationLabel = new("Database Save Location");
        
        private static readonly GUIContent FinalSaveLocationLabel = new("Final Save Location");
        
        #endregion
        
        #region Project File Save Paths

        private static readonly string[] FileSavePaths =
        {
            "Resources",
            "StreamingAssets",
        };
        
        #endregion

        private bool IsAbleToDownload => m_IsGoogleSheetMetadataValid && m_IsFileNameValid && m_Utility.IsCompleted;
        
        #region Editor Window Implementaions
        
        [MenuItem("Tools/Google Sheets Helper")]
        private static void OpenWindow()
        {
            var window = GetWindow<GoogleSheetsHelperEditorWindow>(true, "Google Sheets Helper");
            window.minSize = new Vector2(640, 480);
            window.Show();
        }
        
        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            m_GoogleSheetsURL = EditorGUILayout.DelayedTextField(GoogleSheetsURLLabel, m_GoogleSheetsURL);
            if (EditorGUI.EndChangeCheck())
            {
                m_IsGoogleSheetMetadataValid = GoogleSheetsUtility.TryGetGoogleSheetMeta(m_GoogleSheetsURL, out m_GoogleSheetsKey, out m_SheetGid);
                m_CsvExportURL = GoogleSheetsUtility.GetCsvExportURL(m_GoogleSheetsKey, m_SheetGid);
            }
            
            GUI.enabled = false;
            EditorGUILayout.TextField(SheetKeyLabel, m_GoogleSheetsKey);
            EditorGUILayout.TextField(SheetGIDLabel, m_SheetGid);
            EditorGUILayout.TextField(CsvExportURLLabel, m_CsvExportURL);
            EditorGUILayout.Space();

            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();
            m_DatabaseFileName = EditorGUILayout.DelayedTextField(DatabaseFileNameLabel, m_DatabaseFileName);
            m_SelectedSaveLocationIndex = EditorGUILayout.Popup(SaveFileLocationLabel, m_SelectedSaveLocationIndex, FileSavePaths);
            if (!EditorGUI.EndChangeCheck())
            {
                m_IsFileNameValid = !string.IsNullOrEmpty(m_DatabaseFileName);
                m_FinalSaveLocation = $"{Application.dataPath}/{FileSavePaths[m_SelectedSaveLocationIndex]}/{m_DatabaseFileName}.csv";
                m_IsFileExistsAtGivenLocation = m_IsFileNameValid && File.Exists(m_FinalSaveLocation);
            }

            GUI.enabled = false;
            EditorGUILayout.DelayedTextField(FinalSaveLocationLabel, m_FinalSaveLocation);
            EditorGUILayout.Space();
            EditorGUILayout.ToggleLeft("Is Sheet Metadata Valid", m_IsGoogleSheetMetadataValid);
            EditorGUILayout.ToggleLeft("Is File Name Valid", m_IsFileNameValid);
            EditorGUILayout.ToggleLeft("Is File Exists at Given Location", m_IsFileExistsAtGivenLocation);

            GUI.enabled = IsAbleToDownload;
            if (GUILayout.Button("Download & Save"))
            {
                m_Utility.DownloadFromUrl(m_CsvExportURL);
                GoogleSheetsUtility.SaveCsvFileToPath(m_FinalSaveLocation, m_Utility.Content);
                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
        }
        
        #endregion
    }
}