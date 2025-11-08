using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public GameObject xNoWay; // —сылка на объект X No way

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        if (gameObject.name == "Enemy_Torch")
        {
            Destroy(xNoWay);
        }
    }
}
