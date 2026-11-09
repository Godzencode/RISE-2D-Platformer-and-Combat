using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class Collect : MonoBehaviour
{
    public static Collect instance;
    [SerializeField]private int GreenGem;
    [SerializeField]private GameObject Collect_effect;
    [SerializeField] private Text greenGem_Text;
    public bool hasKey;
    void Start()
    {
        hasKey = false;
        
        if (instance!= null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GreenGem") )
        {
            GreenGem ++;
            greenGem_Text.text = GreenGem.ToString();
            Destroy(collision.gameObject);
            AudioManager.instance.PlaySound("Collect");
            GameObject temp =Instantiate(Collect_effect, collision.gameObject.transform.position, Quaternion.identity);

        }

        if (collision.gameObject.CompareTag("Key"))
        {
            Destroy(collision.gameObject);
            GameObject temp =Instantiate(Collect_effect, collision.gameObject.transform.position, Quaternion.identity);
            Destroy(temp , 0.5f);
            AudioManager.instance.PlaySound("KeyCollect");
            GameManager.instance.TriggerVictoryPanel();
            hasKey = true;

        }
    }
}
