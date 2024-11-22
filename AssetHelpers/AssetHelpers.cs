﻿using System.IO;
using AssetManager.Models;

namespace AssetManager.AssetHelpers
{
    public class AssetHelpers
    {
        public enum AssetType
        {
            Image,
            Model,
            Other
        }

        public enum Tags
        {
            Image,
            Model,
            Document,
            Video,
            Audio
        }

        public static AssetType GetAssetType(string extension)
        {
            return extension.ToLower() switch
            {
                ".png" => AssetType.Image,
                ".jpg" => AssetType.Image,
                ".fbx" => AssetType.Model,
                _ => AssetType.Other
            };
        }

        public static void AddVertexFileInfo(string fileName, Asset asset)
        {
            if(!File.Exists(fileName))
            {
                Console.WriteLine("File does not exist");
                return;
            }

            foreach(var line in File.ReadAllLines(fileName))
            {
                if(line.StartsWith("v"))
                {
                    asset.Metadata.VertexCount++;
                }
                else if(line.StartsWith("f"))
                {
                    asset.Metadata.FaceCount++;
                }
            
            }
        }

        public enum AvailableFormats
        {
            FBX,
            OBJ,
            PNG,
            JPG
        }

      

        public static System.Windows.Media.Color GenerateRandomColorRGB()
        {
            Random random = new Random();
            byte r = (byte)random.Next(128, 256);
            byte g = (byte)random.Next(128, 256);
            byte b = (byte)random.Next(128, 256);

            System.Windows.Media.Color brightColor = System.Windows.Media.Color.FromRgb(r, g, b);

            return brightColor;
        }
    }
}
