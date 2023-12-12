using UnityEngine;

public class AttachChildOnTouch : MonoBehaviour
{
    [SerializeField] private GameObject objectToAttach;


    private void Start()
    {
        objectToAttach = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetObjectToAttach(GameObject objectToAttach)
    {
        this.objectToAttach = objectToAttach;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            objectToAttach.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            objectToAttach.transform.parent = null;
        }
    }
}