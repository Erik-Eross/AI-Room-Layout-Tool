using UnityEngine;

public class IntroScreen : MonoBehaviour
{
    public GameObject createGridScreen;

    public void ContinueButton()
    {
        createGridScreen.SetActive(true);
        gameObject.SetActive(false);
    }
}
