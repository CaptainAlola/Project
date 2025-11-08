using UnityEngine;

public class Win_Lose_Script : MonoBehaviour
{
    public GameObject loseGameObject; // Ссылка на объект Lose
    public GameObject winGameObject; // Ссылка на объект Win

    // Start вызывается один раз перед первым вызовом Update после создания MonoBehaviour
    void Start()
    {
        loseGameObject.SetActive(false); // Убедитесь, что объект Lose изначально неактивен
        winGameObject.SetActive(false); // Убедитесь, что объект Win изначально неактивен
    }

    // Update вызывается один раз за кадр
    void Update()
    {

    }
}