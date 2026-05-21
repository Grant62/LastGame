using System;
using System.Data;
using System.IO;
using System.Text;
using Excel;
using UnityEditor;
using UnityEngine;

namespace Editor.Excel
{
    public class ExcelTool
    {
        public static readonly string ExcelPath = Application.dataPath + "/GameResource/Excel/";
        public static readonly string DataClassPath = Application.dataPath + "/Scripts/Configuration/ExcelData/DataClass/";
        public static readonly string DataContainerPath = Application.dataPath + "/Scripts/Configuration/ExcelData/Container/";
        public static readonly string DataBinaryPath = Application.streamingAssetsPath + "/Binary/";
        public const int BeginIndex = 3;

        [MenuItem("游戏工具/生成Excel配置数据脚本")]
        private static void GenerateExcelInfo()
        {
            ClearGeneratedFiles();
            DirectoryInfo dInfo = Directory.CreateDirectory(ExcelPath);
            FileInfo[] files = dInfo.GetFiles();
            DataTableCollection tableCollection;
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Extension != ".xlsx" &&
                    files[i].Extension != ".xls")
                    continue;

                using (FileStream fs = files[i].Open(FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                    tableCollection = excelReader.AsDataSet().Tables;
                    fs.Close();
                }

                foreach (DataTable table in tableCollection)
                {
                    GenerateExcelDataClass(table);
                    GenerateExcelContainer(table);
                    GenerateExcelBinary(table);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("Excel文件生成完成！");
        }

        private static void ClearGeneratedFiles()
        {
            if (Directory.Exists(DataBinaryPath))
            {
                string[] binaryFiles = Directory.GetFiles(DataBinaryPath, "*.txt");
                foreach (string file in binaryFiles)
                    File.Delete(file);
            }

            if (Directory.Exists(DataClassPath))
            {
                string[] classFiles = Directory.GetFiles(DataClassPath, "*.cs");
                foreach (string file in classFiles)
                    File.Delete(file);
            }

            if (Directory.Exists(DataContainerPath))
            {
                string[] containerFiles = Directory.GetFiles(DataContainerPath, "*.cs");
                foreach (string file in containerFiles)
                    File.Delete(file);
            }
        }

        private static void GenerateExcelDataClass(DataTable table)
        {
            try
            {
                DataRow rowName = GetVariableNameRow(table);
                DataRow rowType = GetVariableTypeRow(table);
                if (!Directory.Exists(DataClassPath))
                    Directory.CreateDirectory(DataClassPath);

                // 检查类型支持
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    string typeName = rowType[i].ToString();
                    if (!IsSupportedType(typeName))
                    {
                        Debug.LogError($"表 {table.TableName} 包含不支持的类型: {typeName}，字段: {rowName[i]}");
                        return;
                    }
                }

                string str = $"namespace Configuration.ExcelData.DataClass\n{{\n    public class " + table.TableName + "\n    {\n";
                for (int i = 0; i < table.Columns.Count; i++)
                    str += "        public " + rowType[i] + " " + rowName[i] + ";\n";
                str += "    }\n}";
                File.WriteAllText(DataClassPath + table.TableName + ".cs", str);
            }
            catch (Exception ex)
            {
                Debug.LogError($"生成数据类 {table.TableName} 失败: {ex.Message}");
            }
        }

        private static bool IsSupportedType(string typeName)
        {
            return typeName == "int" || typeName == "float" || typeName == "bool" || typeName == "string";
        }

        private static void GenerateExcelContainer(DataTable table)
        {
            try
            {
                DataRow rowType = GetVariableTypeRow(table);
                if (!Directory.Exists(DataContainerPath))
                    Directory.CreateDirectory(DataContainerPath);
                string str = $@"using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Services.ExcelTool;

namespace Configuration.ExcelData.Container
{{
    [BinaryTable(DataType = typeof({table.TableName}))]
    public class {table.TableName}Container
    {{
        public Dictionary<{rowType[0]}, {table.TableName}> DataDic = new();
    }}
}}";
                File.WriteAllText(DataContainerPath + table.TableName + "Container.cs", str);
            }
            catch (Exception ex)
            {
                Debug.LogError($"生成容器类 {table.TableName}Container 失败: {ex.Message}");
            }
        }

        private static void GenerateExcelBinary(DataTable table)
        {
            try
            {
                if (!Directory.Exists(DataBinaryPath))
                    Directory.CreateDirectory(DataBinaryPath);

                int validRowCount = 0;
                for (int i = BeginIndex; i < table.Rows.Count; i++)
                {
                    if (IsRowEmpty(table.Rows[i]))
                        continue;
                    validRowCount++;
                }

                using (FileStream fs = new(DataBinaryPath + table.TableName + ".txt", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(BitConverter.GetBytes(validRowCount), 0, 4);
                    string keyName = GetVariableNameRow(table)[0].ToString();
                    byte[] bytes = Encoding.UTF8.GetBytes(keyName);
                    fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                    fs.Write(bytes, 0, bytes.Length);
                    DataRow row;
                    DataRow rowType = GetVariableTypeRow(table);
                    for (int i = BeginIndex; i < table.Rows.Count; i++)
                    {
                        row = table.Rows[i];
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            string cellValue = row[j].ToString();
                            switch (rowType[j].ToString())
                            {
                                case "int":
                                    if (string.IsNullOrEmpty(cellValue))
                                        fs.Write(BitConverter.GetBytes(0), 0, 4);
                                    else
                                        fs.Write(BitConverter.GetBytes(int.Parse(cellValue)), 0, 4);
                                    break;
                                case "float":
                                    if (string.IsNullOrEmpty(cellValue))
                                        fs.Write(BitConverter.GetBytes(0f), 0, 4);
                                    else
                                        fs.Write(BitConverter.GetBytes(float.Parse(cellValue)), 0, 4);
                                    break;
                                case "bool":
                                    if (string.IsNullOrEmpty(cellValue))
                                        fs.Write(new byte[] { 0 }, 0, 1);
                                    else
                                        fs.Write(BitConverter.GetBytes(bool.Parse(cellValue)), 0, 1);
                                    break;
                                case "string":
                                    if (string.IsNullOrEmpty(cellValue))
                                        fs.Write(BitConverter.GetBytes(0), 0, 4);
                                    else
                                    {
                                        bytes = Encoding.UTF8.GetBytes(cellValue);
                                        fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                                        fs.Write(bytes, 0, bytes.Length);
                                    }

                                    break;
                                default:
                                    Debug.LogWarning($"表 {table.TableName} 行 {i} 列 {j} 包含未知类型: {rowType[j]}");
                                    break;
                            }
                        }
                    }

                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"生成二进制文件 {table.TableName}.txt 失败: {ex.Message}");
            }
        }

        private static bool IsRowEmpty(DataRow row)
        {
            if (row == null)
                return true;

            foreach (object item in row.ItemArray)
            {
                if (!string.IsNullOrWhiteSpace(item?.ToString()))
                    return false;
            }

            return true;
        }

        private static DataRow GetVariableNameRow(DataTable table)
        {
            return table.Rows[0];
        }

        private static DataRow GetVariableTypeRow(DataTable table)
        {
            return table.Rows[1];
        }
    }
}