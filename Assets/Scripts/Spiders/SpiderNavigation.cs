using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderNavigation : MonoBehaviour
{
    public GameObject destinationParent;
    private GameObject[] destinations;
    private NavMeshAgent spider;
    private int destinationCount;

    // Start is called before the first frame update
    void Start()
    {
        destinations = new GameObject[destinationParent.transform.childCount];

        for(int i = 0; i < destinationParent.transform.childCount; ++i)
        {
            destinations[i] = destinationParent.transform.GetChild(i).gameObject;
        }
        spider = this.GetComponent<NavMeshAgent>();

        GoDestination();
    }

    private void GoDestination()
    {
        spider.destination = destinations[destinationCount].transform.position;
        StartCoroutine(WaitUntilReached());
    }

    private IEnumerator WaitUntilReached()
    {
        while(Mathf.Abs(this.transform.position.x - destinations[destinationCount].transform.position.x) > 0.2f && Mathf.Abs(this.transform.position.z - destinations[destinationCount].transform.position.z) > 0.2f)
        {
            //wait until its there
            yield return null;
        }
        destinationCount++;
        if (destinationCount < destinations.Length)
        {
            GoDestination();
        }
        else
        {
            Debug.Log("all destinations reached!");
        }

    }
}
