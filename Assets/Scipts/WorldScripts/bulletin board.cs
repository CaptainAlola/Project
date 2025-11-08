using UnityEngine;

public class BoardAreaTrigger : MonoBehaviour
{
    public GameObject board; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            if (board != null)
            {
                board.SetActive(true); 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            if (board != null)
            {
                board.SetActive(false); 
            }
        }
    }
}