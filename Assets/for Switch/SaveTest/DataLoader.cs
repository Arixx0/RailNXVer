using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    [SerializeField] Stack<string> saveDataLine = new Stack<string>();
    [SerializeField] Queue<string> loadQue = new Queue<string>();
    string savedata;
    float dealy = 1.1f;
    float saveTimer = 0f;
    float loadTimer = 1f;
    
    void Start()
    {
         
    }

    
    void Update()
    {
        Save();
    }

    public void Save()
    {
        if(saveTimer > dealy && saveDataLine.Count > 0)
        {
            saveTimer = 0f;
            string savedatatemp = saveDataLine.Pop();
            XmlSerializer xmlfile = new XmlSerializer(typeof(string));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlfile.Serialize(memoryStream, savedatatemp);

                // 바이너리 파일로 저장
                byte[] xmlData = memoryStream.ToArray();
                File.WriteAllBytes("savedata.binary", xmlData);
            }
        }
        else if(saveTimer < dealy)
        {
            saveTimer += Time.deltaTime;
        }
    }

    int clickcount = 0;
    private void sampleTest()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            saveDataLine.Push(clickcount.ToString());
            clickcount++;
        }
    }
}
