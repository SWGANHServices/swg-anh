using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void FactionSelectionMenu() //=> SceneManager.LoadScene("RaceSelection", LoadSceneMode.Single);
    {
        SceneManager.LoadScene("RealmSelection", LoadSceneMode.Single);
    }
    public void CharacterSelectionMenu() //=> SceneManager.LoadScene("CharacterSelection", LoadSceneMode.Single);
    {
        SceneManager.LoadScene("CharacterSelection", LoadSceneMode.Single);
    }
    public void CharacterCreationMenu() //=> SceneManager.LoadScene("CharacterCreation", LoadSceneMode.Single);
    {
        SceneManager.LoadScene("CharacterCreation", LoadSceneMode.Single);
    }
}