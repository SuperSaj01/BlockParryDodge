using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Services.Core;

public class TestRelay : MonoBehaviour
{
    private async void Start()
    {
        await UnityServices.InitializeAsync();


        AuthencticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in:" + AuthencticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void CreateRelay()
    {
        try{
        await RelayService.Instance.CreateAllocationAsync(1);
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
