using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{

    public RectTransform backgroundImage;
    [SerializeField] private float scrollingSpeed = 0.01f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BackgroundScrolling();
    }

    private void BackgroundScrolling()
    {
        // Debug.LogError(backgroundImage.localPosition);
        if (backgroundImage.localPosition.y > 390.1f)
        {
            backgroundImage.localPosition = new Vector3(0, -432, 0);
        }
        else
        {
            backgroundImage.localPosition += new Vector3(0, scrollingSpeed, 0);
        }
    }
    
    public void TransferToNextLevel()
    {
        int currentScneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScneIndex + 1);
    }
}
