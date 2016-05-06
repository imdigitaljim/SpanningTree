using UnityEngine;
using System.Collections;

public class FeedBackManager : MonoBehaviour
{

    public float Timeout;
    private float _current;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    _current += Time.deltaTime;
	    if (_current >= Timeout)
	    {
	        gameObject.SetActive(false);
	    }
	}

    public void ResetTime()
    {
        _current = 0;
    }
}
