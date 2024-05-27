using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ExcelImporter : AssetPostprocessor
{
    private class ExcelAssetInfo
    {
        public Type AssetType { get; set; }
        public ExcelAssetAttribute Attribute { get; set; }
        public string ExcelName => string.IsNullOrEmpty(Attribute.ExcelName) ? AssetType.Name : Attribute.ExcelName;
    }

    private static List<ExcelAssetInfo> cachedInfos; // Clear on compile.

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        var imported = false;
        foreach (var path in importedAssets)
            if (Path.GetExtension(path) == ".xls" || Path.GetExtension(path) == ".xlsx")
            {
                if (cachedInfos == null) cachedInfos = FindExcelAssetInfos();

                var excelName = Path.GetFileNameWithoutExtension(path);
                if (excelName.StartsWith("~$")) continue;

                var info = cachedInfos.Find(i => i.ExcelName == excelName);

                if (info == null) continue;

                ImportExcel(path, info);
                imported = true;
            }

        if (imported)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private static List<ExcelAssetInfo> FindExcelAssetInfos()
    {
        var list = new List<ExcelAssetInfo>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        foreach (var type in assembly.GetTypes())
        {
            var attributes = type.GetCustomAttributes(typeof(ExcelAssetAttribute), false);
            if (attributes.Length == 0) continue;
            var attribute = (ExcelAssetAttribute)attributes[0];
            var info = new ExcelAssetInfo
            {
                AssetType = type,
                Attribute = attribute
            };
            list.Add(info);
        }

        return list;
    }

    private static Object LoadOrCreateAsset(string assetPath, Type assetType)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(assetPath));

        var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance(assetType.Name);
            AssetDatabase.CreateAsset((ScriptableObject)asset, assetPath);
            asset.hideFlags = HideFlags.NotEditable;
        }

        return asset;
    }

    private static IWorkbook LoadBook(string excelPath)
    {
        using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            if (Path.GetExtension(excelPath) == ".xls") return new HSSFWorkbook(stream);
            return new XSSFWorkbook(stream);
        }
    }

    private static List<string> GetFieldNamesFromSheetHeader(ISheet sheet)
    {
        var headerRow = sheet.GetRow(0);

        var fieldNames = new List<string>();
        for (var i = 0; i < headerRow.LastCellNum; i++)
        {
            var cell = headerRow.GetCell(i);
            if (cell == null || cell.CellType == CellType.Blank) break;
            fieldNames.Add(cell.StringCellValue);
        }

        return fieldNames;
    }

    private static object CellToFieldObject(ICell cell, FieldInfo fieldInfo, bool isFormulaEvalute = false)
    {
        var type = isFormulaEvalute ? cell.CachedFormulaResultType : cell.CellType;

        switch (type)
        {
            case CellType.String:
                if (fieldInfo.FieldType.IsEnum) return Enum.Parse(fieldInfo.FieldType, cell.StringCellValue);
                return cell.StringCellValue;
            case CellType.Boolean:
                return cell.BooleanCellValue;
            case CellType.Numeric:
                return Convert.ChangeType(cell.NumericCellValue, fieldInfo.FieldType);
            case CellType.Formula:
                if (isFormulaEvalute) return null;
                return CellToFieldObject(cell, fieldInfo, true);
            default:
                if (fieldInfo.FieldType.IsValueType) return Activator.CreateInstance(fieldInfo.FieldType);
                return null;
        }
    }

    private static object CreateEntityFromRow(IRow row, List<string> columnNames, Type entityType, string sheetName)
    {
        var entity = Activator.CreateInstance(entityType);

        for (var i = 0; i < columnNames.Count; i++)
        {
            var entityField = entityType.GetField(
                columnNames[i],
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
            if (entityField == null) continue;
            if (!entityField.IsPublic &&
                entityField.GetCustomAttributes(typeof(SerializeField), false).Length == 0) continue;

            var cell = row.GetCell(i);
            if (cell == null) continue;

            try
            {
                var fieldValue = CellToFieldObject(cell, entityField);
                entityField.SetValue(entity, fieldValue);
            }
            catch
            {
                throw new Exception(string.Format("Invalid excel cell type at row {0}, column {1}, {2} sheet.",
                    row.RowNum, cell.ColumnIndex, sheetName));
            }
        }

        return entity;
    }

    private static object GetEntityListFromSheet(ISheet sheet, Type entityType)
    {
        var excelColumnNames = GetFieldNamesFromSheetHeader(sheet);

        var listType = typeof(List<>).MakeGenericType(entityType);
        var listAddMethod = listType.GetMethod("Add", new[] { entityType });
        var list = Activator.CreateInstance(listType);

        // row of index 0 is header
        for (var i = 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            if (row == null) break;

            var entryCell = row.GetCell(0);
            if (entryCell == null || entryCell.CellType == CellType.Blank) break;

            // skip comment row
            if (entryCell.CellType == CellType.String && entryCell.StringCellValue.StartsWith("#")) continue;

            var entity = CreateEntityFromRow(row, excelColumnNames, entityType, sheet.SheetName);
            listAddMethod.Invoke(list, new[] { entity });
        }

        return list;
    }

    private static void ImportExcel(string excelPath, ExcelAssetInfo info)
    {
        var assetPath = "";
        var assetName = info.AssetType.Name + ".asset";

        if (string.IsNullOrEmpty(info.Attribute.AssetPath))
        {
            var basePath = Path.GetDirectoryName(excelPath);
            assetPath = Path.Combine(basePath, assetName);
        }
        else
        {
            var path = Path.Combine("Assets", info.Attribute.AssetPath);
            assetPath = Path.Combine(path, assetName);
        }

        var asset = LoadOrCreateAsset(assetPath, info.AssetType);

        var book = LoadBook(excelPath);

        var assetFields = info.AssetType.GetFields();
        var sheetCount = 0;

        foreach (var assetField in assetFields)
        {
            var sheet = book.GetSheet(assetField.Name);
            if (sheet == null) continue;

            var fieldType = assetField.FieldType;
            if (!fieldType.IsGenericType || fieldType.GetGenericTypeDefinition() != typeof(List<>)) continue;

            var types = fieldType.GetGenericArguments();
            var entityType = types[0];

            var entities = GetEntityListFromSheet(sheet, entityType);
            assetField.SetValue(asset, entities);
            sheetCount++;
        }

        if (info.Attribute.LogOnImport)
            Debug.Log(string.Format("Imported {0} sheets form {1}.", sheetCount, excelPath));

        EditorUtility.SetDirty(asset);
    }
}