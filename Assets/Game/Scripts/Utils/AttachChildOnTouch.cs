using UnityEngine;

public class AttachChildOnTouch : MonoBehaviour
{
    [SerializeField] private GameObject objectToAttach;


    private void Awake()
    {
        objectToAttach = GameObject.FindGameObjectWithTag("Player");
    }

    public GameObject GetObjectToAttach()
    {
        return objectToAttach;
    }

    public void SetObjectToAttach(GameObject objectToAttach)
    {
        this.objectToAttach = objectToAttach;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
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