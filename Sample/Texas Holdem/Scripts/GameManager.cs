using System.Threading.Tasks;
using R3;
using R3.Triggers;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        //GameObject mainMenuWindow = Instantiate(Resources.Load<GameObject>("Prefabs/Windows/UIMainMenuWindow"));
        GameObject mainMenuWindow = GameObject.Find("MainMenuWindow");
        UIMainMenuWindow uIMainMenuWindow = new UIMainMenuWindow();
        uIMainMenuWindow.Init(mainMenuWindow);
        Observable.FromAsync(async ct => await uIMainMenuWindow.Transition("Show"))
            .Subscribe(result => Debug.Log(result));
    }

    // Update is called once per frame
    void OnEnable()
    {
        Debug.Log("OnEnable");
    }
    void Update()
    {
        
    }
}
