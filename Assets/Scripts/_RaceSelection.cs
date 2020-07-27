using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public class RaceSelection : MonoBehaviour
{
    private readonly ConfigurationService configService;
    //private readonly IPackageParser packageParser;
    //private readonly NetworkConnection clientConn;
    private readonly MenuManager menuManager;

    public RaceSelection()
    {
        configService = ConfigurationService.CreateInstance(s =>
        {
            //s.AddSingleton<IPackageParser, PackageParser>();
        });
        //packageParser = configService.ServiceProvider.GetRequiredService<IPackageParser>();
        menuManager = new MenuManager();
    }

    //public void RaceOne()
    //{
    //    clientConn = new ClientConnection();
    //    Debug.Log("Write Race packages");
    //    int RaceID = 1;

    //    packageParser.ParsePackgeToStream(new RaceRequestPackage
    //    {
    //        RaceID = RaceID,

    //    }, clientConn.Writer);

    //    Debug.Log("Receive Login response packages");
    //    var packgeData = packageParser.ParsePackageFromStream(clientConn.Reader);
    //    //Debug.Log($"Receive Login response packages TYPE: {packgeData.GetType()} RESULT: {(packgeData as RaceRequestPackage)?. IsValid}");

    //    menuManager.CharacterCreationMenu();

    //}

    public void RaceTwo()
    {
    }

    public void RaceThree()
    {
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}