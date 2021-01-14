using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.UI;

public class TradeButton : MonoBehaviour
{
    [SerializeField]
    protected TradeWindow TradeWindow;

    public void Awake()
    {
        Button.ButtonClickedEvent click = new Button.ButtonClickedEvent();
        click.AddListener(this.Click);
        this.GetComponent<Button>().onClick = click;
    }
    
    protected void Click()
    {
        this.TradeWindow.Trade();
    }
}
