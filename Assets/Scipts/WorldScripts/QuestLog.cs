using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class QuestLog : MonoBehaviour
{
    public GameObject quest1;
    public GameObject quest2;
    public GameObject quest2_2;
    public GameObject quest3;
    public GameObject quest3_3;
    public Text questLogText; 

    void Update()
    {
        string log = "      Active quest:\n";

        if (quest1 != null && quest1.activeInHierarchy)
            log += "        -Quest1\nподойти к странному человеку";
        if (quest2_2 != null && quest2.activeInHierarchy)
            log += "-Quest2\nсобрать грибы, но где?" ;
        if (quest3 != null && quest2_2.activeInHierarchy)
            log += "-Quest2.2\n";
        if (quest3 != null && quest3.activeInHierarchy)
            log += "-Quest3\nУспокоить кр рыдцаря";
        if (quest3_3 != null && quest3_3.activeInHierarchy)
            log += "-Quest3.3\n";

        questLogText.text = log;
    }
}
