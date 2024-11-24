using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AssetManager.AssetHelpers
{
    public class FileConverter
    {
        static private string RunConversionTool(string converterName, string inputFilePath, string outputFilePath)
        {
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", converterName, $"{converterName}.exe");
            string outputFolderPath = Path.GetDirectoryName(outputFilePath);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{inputFilePath}\" \"{outputFolderPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    process.WaitForExit();

                    if (process.ExitCode == 0 && File.Exists(outputFilePath))
                    {
                        return outputFilePath;
                    }
                }
            }
            return null;
        }

        static public string ConvertFbxToObj(string fbxFilePath, string outputFolderPath = "")
        {
            if (string.IsNullOrEmpty(outputFolderPath))
            {
                outputFolderPath = Path.GetDirectoryName(fbxFilePath);
            }

            string outputFilePath = Path.Combine(outputFolderPath, Path.GetFileName(Path.ChangeExtension(Path.GetFileName(fbxFilePath), ".obj")));
            return RunConversionTool("FBXToObjConverter", fbxFilePath, outputFilePath);
        }

        static public string ConvertObjToFbx(string objFilePath)
        {
            string outputFilePath = Path.ChangeExtension(objFilePath, ".fbx");
            return RunConversionTool("ObjToFbxConverter", objFilePath, outputFilePath);
        }

        static public string ConvertPngToJpg(string pngFilePath)
        {
            string outputFilePath = Path.ChangeExtension(pngFilePath, ".jpg");
            return RunConversionTool("PngToJpgConverter", pngFilePath, outputFilePath);
        }

        static public string ConvertJpgToPng(string jpgFilePath)
        {
            string outputFilePath = Path.ChangeExtension(jpgFilePath, ".png");
            return RunConversionTool("JpgToPngConverter", jpgFilePath, outputFilePath);
        }
    }
}
