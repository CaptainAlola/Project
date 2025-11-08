using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public GameObject winGameObject;

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger area");
            winGameObject.SetActive(true);
            Time.timeScale = 0f; 
        }
    }
}