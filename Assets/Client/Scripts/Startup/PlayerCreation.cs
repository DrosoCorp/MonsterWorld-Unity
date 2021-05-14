using MonsterWorld.Unity.Network;
using MonsterWorld.Unity.Network.Client;
using System;
using TMPro;
using UnityEngine;

public class PlayerCreation : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;

    [SerializeField] private TextMeshProUGUI errorText;

    public Action OnCreationSuccess;

    public void OnButtonCreate()
    {
        var playerCreationPacket = new PlayerCreationPacket()
        {
            name = nameField.text
        };
        ClientNetworkManager.OnPlayerCreationResponsePacket += OnResponsePacket;
        ClientNetworkManager.SendPacket(ref playerCreationPacket);
    }

    private void OnResponsePacket(ref PlayerCreationResponsePacket packet)
    {
        if (packet.responseType == PlayerCreationResponsePacket.ResponseType.Success)
        {
            ClientNetworkManager.OnPlayerCreationResponsePacket -= OnResponsePacket;
            if (OnCreationSuccess != null) OnCreationSuccess();
        }
        else
        {
            ShowError(packet.responseType);
        }
    }

    private void ShowError(PlayerCreationResponsePacket.ResponseType responseType)
    {
        switch (responseType)
        {
            case PlayerCreationResponsePacket.ResponseType.UsernameAlreadyTaken:
                errorText.text = "This name is already taken !";
                break;
            case PlayerCreationResponsePacket.ResponseType.PlayerAlreadyExists:
                errorText.text = "Error : Player Already Exists";
                break;
            case PlayerCreationResponsePacket.ResponseType.ClientNotAuthenticated:
                errorText.text = "Error : Client Not Authenticated";
                break;
            case PlayerCreationResponsePacket.ResponseType.InvalidPayload:
                errorText.text = "Server Error";
                break;
        }
    }
}
