using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartSceneController : MonoBehaviour
{

    public GameObject energyBall;

    private RedHollowControl animationController;
    // Start is called before the first frame update
    void Start()
    {
        energyBall.transform.localScale = Vector3.one * 2;
        animationController = energyBall.GetComponent<RedHollowControl>();
        animationController.Finish_Charging();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
