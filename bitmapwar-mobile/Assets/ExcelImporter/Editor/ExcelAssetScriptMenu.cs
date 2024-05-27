using System;
using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ExcelAssetScriptMenu
{
    private const string ScriptTemplateName = "ExcelAssetScriptTemplete.cs.txt";

    private const string FieldTemplete =
        "\t//public List<EntityType> #FIELDNAME#; // Replace 'EntityType' to an actual type that is serializable.";

    [MenuItem("Assets/Create/ExcelAssetScript", false)]
    private static void CreateScript()
    {
        var savePath = EditorUtility.SaveFolderPanel("Save ExcelAssetScript", Application.dataPath, "");
        if (savePath == "") return;

        var selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

        var excelPath = AssetDatabase.GetAssetPath(selectedAssets[0]);
        var excelName = Path.GetFileNameWithoutExtension(excelPath);
        var sheetNames = GetSheetNames(excelPath);

        var scriptString = BuildScriptString(excelName, sheetNames);

        var path = Path.ChangeExtension(Path.Combine(savePath, excelName), "cs");
        File.WriteAllText(path, scriptString);

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Create/CreateScriptValidation", true)]
    private static bool CreateScriptValidation()
    {
        var selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        if (selectedAssets.Length != 1) return false;
        var path = AssetDatabase.GetAssetPath(selectedAssets[0]);
        return Path.GetExtension(path) == ".xls" || Path.GetExtension(path) == ".xlsx";
    }

    private static List<string> GetSheetNames(string excelPath)
    {
        var sheetNames = new List<string>();
        using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            IWorkbook book = null;
            if (Path.GetExtension(excelPath) == ".xls") book = new HSSFWorkbook(stream);
            else book = new XSSFWorkbook(stream);

            for (var i = 0; i < book.NumberOfSheets; i++)
            {
                var sheet = book.GetSheetAt(i);
                sheetNames.Add(sheet.SheetName);
            }
        }

        return sheetNames;
    }

    private static string GetScriptTempleteString()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var filePath = Directory.GetFiles(currentDirectory, ScriptTemplateName, SearchOption.AllDirectories);
        if (filePath.Length == 0) throw new Exception("Script template not found.");

        var templateString = File.ReadAllText(filePath[0]);
        return templateString;
    }

    private static string BuildScriptString(string excelName, List<string> sheetNames)
    {
        var scriptString = GetScriptTempleteString();

        scriptString = scriptString.Replace("#ASSETSCRIPTNAME#", excelName);

        foreach (var sheetName in sheetNames)
        {
            var fieldString = string.Copy(FieldTemplete);
            fieldString = fieldString.Replace("#FIELDNAME#", sheetName);
            fieldString += "\n#ENTITYFIELDS#";
            scriptString = scriptString.Replace("#ENTITYFIELDS#", fieldString);
        }

        scriptString = scriptString.Replace("#ENTITYFIELDS#\n", "");

        return scriptString;
    }
}