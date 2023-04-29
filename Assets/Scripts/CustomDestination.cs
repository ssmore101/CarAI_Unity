using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CustomDestination : MonoBehaviour
{
    [SerializeField] private AIDriver aiDriver;
    [SerializeField] private TMP_Dropdown dropdown;

    private void Start()
    {
        // Create dropdown options list
        List<string> options = new List<string>();
        options.Add("House");
        options.Add("SuperMarket");
        options.Add("Bank");
        options.Add("Police Station");
        options.Add("Barber Shop");
        options.Add("Doughnut Shop");
        options.Add("Bar");
        options.Add("Hospital");
        options.Add("CityHall");

        // Set dropdown options
        dropdown.ClearOptions();
        dropdown.AddOptions(options);

        // Add listener to dropdown
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {
        // Go to custom destination
        aiDriver.GoToCustomDestination(index);
    }
}
