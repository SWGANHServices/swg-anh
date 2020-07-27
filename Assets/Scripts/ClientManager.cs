using Assets.Scripts;
using Microsoft.Extensions.DependencyInjection;
using SWGANH_Core;
using SWGANH_Core.Client;
using SWGANH_Core.PackageParser;
using SWGANH_Core.PackageParser.PackageImplimentations;
using System.Collections;
using System.Collections.Generic;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Debug;

public class ClientManager : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;

    //private readonly IPackageParser packageParser; //replaced by connectionDispatcher.
    private readonly NetworkConnection clientconn;
    private readonly ClientConnectionDispatcher connectionDispatcher;
    private readonly MenuManager menuManager;

    [Header("Character Creation")]
    public DynamicCharacterAvatar characterAvatar;
    public InputField CharacterNameInput;

    public static string RealmIDtext { get; set; }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    public ClientManager()
    {
        clientconn = GameContext.ServicesProvider.GetRequiredService<NetworkConnection>();

        connectionDispatcher = GameContext.ServicesProvider.GetRequiredService<ClientConnectionDispatcher>();
        //packageParser = GameContext.ServicesProvider.GetRequiredService<IPackageParser>(); // replaced bu connectionDispatcher
    }

    public void Execute()
    {
        Log("Build Connection");
        var t = GetComponentInChildren<MenuManager>(true);

        clientconn.Connect("127.0.0.1", 3456);

        connectionDispatcher.Start();
        Log("Write Login packages");
        string username = usernameInput.text;
        string password = passwordInput.text;


        connectionDispatcher.SendPackage(new LoginRequestPackage
        {
            Username = username,
            Password = password
        });

        Log("Receive Login responce Package");
        var packageData = connectionDispatcher.WaitForPackage<LoginResponsePackage>().Result;
        Log($"Receive login responce package type: {packageData.GetType()} Result: {packageData?.IsValid}");
        t.FactionSelectionMenu();
    }

    public void FactionOne()
    {
        Log("Write Race Packages");
        int _realmID = 1;
        connectionDispatcher.SendPackage(new RealmRequestPackage
        {
            RealmID = _realmID,
        });

        Log("Receive Realm Response Package");

        //var packageData = packageParser.ParserPackageFromStream(clientconn.Reader);
        var packageData = connectionDispatcher.WaitForPackage<RealmResponsePackage>().Result;
        Log($"Recieve Realm Responce package Type: {packageData.GetType()} RESULT: {packageData?.RealmID}");

        RealmIDtext = packageData?.RealmID.ToString();

        var CharExists = false;

        var t = GetComponentInChildren<MenuManager>(true);

        if(CharExists)
        {
            t.CharacterSelectionMenu();
        }
        else
        {
            t.CharacterCreationMenu();
        }
    }

    public string UmaRecipeData { get; set; }
    public void SaveCharacter()
    {
        string UmaRecipeData = characterAvatar.GetCurrentColorsRecipe();
        string CharacterName = CharacterNameInput.text;

        connectionDispatcher.SendPackage(new CharacterCreationRequestPackage
        {
            CharName = CharacterName,
            UmaRecipe = UmaRecipeData,
        });
    }
}