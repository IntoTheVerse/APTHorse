using Aptos.HdWallet;
using Aptos.Unity.Rest;
using Aptos.Unity.Sample.UI;
using TMPro;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI AptosTokenBalance;
    [SerializeField] private TextMeshProUGUI AptosAddress;
    [SerializeField] private TextMeshProUGUI AptosUsername;

    [HideInInspector] public Wallet Wallet = null;
    [HideInInspector] public float APTBalance;
    [HideInInspector] public string Username = "";
    [HideInInspector] public int EquippedHorseId = 1000;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        AptosUILink.Instance.onGetBalance += val =>
        {
            APTBalance = val;
            float bal = AptosUILink.Instance.AptosTokenToFloat(val);
            AptosTokenBalance.text = $"APT Balance : {bal}";
            if (bal < 10) StartCoroutine(AptosUILink.Instance.AirDrop(1000000000));
        };
        RestClient.Instance.SetEndPoint(Constants.RANDOMNET_BASE_URL);
    }

    public bool AuthenticateWithWallet()
    {
        if (Wallet != null) return true;

        string mneomicsKey = PlayerPrefs.GetString(AptosUILink.Instance.mnemonicsKey, "");
        if (string.IsNullOrEmpty(mneomicsKey))
        {
            if (!AptosUILink.Instance.CreateNewWallet()) return false;
            mneomicsKey = PlayerPrefs.GetString(AptosUILink.Instance.mnemonicsKey, "");
        }
        else if (!AptosUILink.Instance.RestoreWallet(mneomicsKey)) return false;

        Wallet = new Wallet(mneomicsKey);
        AptosAddress.text = $"Address : {AptosUILink.Instance.GetCurrentWalletAddress()}";

        return true;
    }

    public void UpdateUsername()
    {
        AptosUsername.text = Username;
    }
}
