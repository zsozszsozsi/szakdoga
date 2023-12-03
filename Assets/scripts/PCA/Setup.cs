using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Setup : MonoBehaviour
{
    private int DataSet;
    private int Dimension;

    public GameObject DropDown;
    public ToggleGroup DimensionToggleGroup;
    public ToggleGroup SamplesToggleGroup;
    public ToggleGroup LabelPosToggleGroup;
    public GameObject InputField;

    private Dictionary<int, string> DropDownSelect = new();
    private Dictionary<string, int> DimensionToggle = new() { { "2D", 2 }, { "3D", 3 } };
    private int SelectedDataSet = 0;
    private int SelectedDimension = 3;
    private LoadData.LabelPos SelectedLabelPos = LoadData.LabelPos.First;

    // Start is called before the first frame update
    void Start()
    {
        int key = 0;
        foreach(var option in DropDown.GetComponent<TMP_Dropdown>().options)
        {
            DropDownSelect.Add(key++, option.text);
        }
    }
    
    public void HandleInputData(int val)
    {
        SelectedDataSet = val;
        if (DropDownSelect[SelectedDataSet].ToLower().StartsWith("other"))
        {
            InputField.SetActive(true);
            LabelPosToggleGroup.gameObject.SetActive(true);
        }
        else
        {
            InputField.SetActive(false);
            LabelPosToggleGroup.gameObject.SetActive(false);
        }


        if (!DropDownSelect[SelectedDataSet].ToLower().StartsWith("mnist"))
        {
            SamplesToggleGroup.gameObject.SetActive(false);
        }
        else
        {
            SamplesToggleGroup.gameObject.SetActive(true);
        }


    }
    
    public void BtnStart()
    {
        SelectedDimension = DimensionToggle[DimensionToggleGroup.ActiveToggles().FirstOrDefault().GetComponentInChildren<Text>().text];
        int samples = -1;

        string path = "";

        if (DropDownSelect[SelectedDataSet].ToLower().StartsWith("other"))
        {
            SelectedLabelPos = LabelPosToggleGroup.ActiveToggles().FirstOrDefault()
                .GetComponentInChildren<Text>().text.ToLower() == "first" ? LoadData.LabelPos.First : LoadData.LabelPos.Last;

            path = InputField.GetComponentInChildren<TMP_InputField>().text;
            GetComponent<LoadData>().StartProcessing(path, SelectedDimension, SelectedLabelPos);
        }
        else
        {
            path = "Assets/DataSets/";

            if (DropDownSelect[SelectedDataSet].ToLower().StartsWith("mnist"))
            {
                samples = int.Parse(SamplesToggleGroup.ActiveToggles().FirstOrDefault().GetComponentInChildren<Text>().text);
                path += DropDownSelect[SelectedDataSet].Split(" ")[0].ToLower() + "_" + samples + ".csv";
                GetComponent<LoadData>().StartProcessing(path, SelectedDimension, LoadData.LabelPos.First);
            }
            else if(DropDownSelect[SelectedDataSet].ToLower().StartsWith("iris"))
            {
                path += DropDownSelect[SelectedDataSet].Split(" ")[0].ToLower() + ".csv";
                GetComponent<LoadData>().StartProcessing(path, SelectedDimension, LoadData.LabelPos.Last);
            }
            
        }

        Debug.Log($"DataSet: {path}, dimension: {SelectedDimension}, samples: {samples}");
    }
}
