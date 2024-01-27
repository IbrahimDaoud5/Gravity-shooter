using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Register : MonoBehaviour
{
    public TMP_InputField usernameInputField, passwordInputField, confirmPasswordInputField;
    public Text label;
    public Button createButton;

    void Start()
    {
        usernameInputField.onValueChanged.AddListener(delegate { HideErrorMessage(); });
        passwordInputField.onValueChanged.AddListener(delegate { HideErrorMessage(); });
        confirmPasswordInputField.onValueChanged.AddListener(delegate { HideErrorMessage(); });
    }
    void Update()
    {

    }

    public void CallRegister()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;
        label.text = "";
        label.color = Color.red;
        LoginData loginData = new LoginData(username, password);
        string jsonData = JsonUtility.ToJson(loginData);


        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            Debug.Log("Required INFO MISSING !");
            label.text = "Please fill in the required INFO";//SHOW IN LABEL
        }
        else if (password != confirmPassword)
        {
            string msg = "Passwords do NOT match!";
            Debug.Log(msg);
            label.text = msg;//SHOW IN LABEL

        }
        else ServerRequestHandler.Instance.SendRequest("/register", jsonData, HandleResponse);
    }

    private void HandleResponse(string responseText)
    {
        if (responseText == null)
        {
            // Handle error
            return;
        }
        else if (responseText != "Registration successful")
            label.text = responseText;//SHOW IN LABEL
        else  // ---> Registration successful
        {
            string s = "You've successfully signed up, Please Log in";
            label.color = Color.green;
            label.text = s;//SHOW IN LABEL
        }
        // Process the response
        // Additional response handling logic...
    }
    public void HideErrorMessage()
    {
        label.text = "";
    }
}