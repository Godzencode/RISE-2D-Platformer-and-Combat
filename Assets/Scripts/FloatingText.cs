using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public static FloatingText instance;
    [SerializeField] private  TextMesh textMesh;

    void Start()
    {
        int randomNum = Random.Range(1, 101);
        textMesh.text = randomNum.ToString();
        Destroy(gameObject , 0.9f);
    }

    

}
