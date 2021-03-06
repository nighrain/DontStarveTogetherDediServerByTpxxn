﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 饥荒开服工具ByTpxxn.Class.Tools
{
    internal static class CommonPath
    {
        #region 字段、属性
        
        /// <summary>
        /// 存档根目录
        /// </summary>
        public static string SaveRootDirPath { get; set; }

        /// <summary>
        /// 游戏平台
        /// </summary>
        private static string _gamePlatform;

        public static string GamePlatform
        {
            get => _gamePlatform;
            set
            {
                WriteGamePlatform(value);
                switch (value)
                {
                    case "Steam":
                        SaveRootDirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Klei\DoNotStarveTogether";
                        break;
                    case "WeGame":
                        SaveRootDirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Klei\DoNotStarveTogetherRail";
                        break;
                }
                _gamePlatform = value;
            }
        }

        /// <summary>
        /// 客户端exe文件路径
        /// </summary>
        private static string _clientFilePath;

        public static string ClientFilePath
        {
            get => _clientFilePath;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _clientFilePath = null;
                    return;
                }
                _clientFilePath = value.Trim();
                WriteClientPath(_clientFilePath, ReadGamePlatform());
                ClientModsDirPath = _clientFilePath.Substring(0, _clientFilePath.Length - 25) + "\\mods";
            }
        }

        /// <summary>
        /// 客户端mods路径
        /// </summary>
        public static string ClientModsDirPath { get; set; }

        /// <summary>
        /// 服务端exe文件路径
        /// </summary>
        private static string _serverFilePath;

        public static string ServerFilePath
        {
            get => _serverFilePath;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _serverFilePath = null;
                    return;
                }
                // 判断文件名对不对
                if (value.Contains("dontstarve_dedicated_server_nullrenderer.exe"))
                {
                    _serverFilePath = value.Trim();
                    WriteServerPath(_serverFilePath, ReadGamePlatform());
                    ServerModsDirPath = _serverFilePath.Substring(0, _serverFilePath.Length - 49) + "\\mods";
                }
            }
        }

        /// <summary>
        /// 服务器mods路径
        /// </summary>
        public static string ServerModsDirPath { get; set; }

        /// <summary>
        /// ClusterToken[令牌]
        /// </summary>
        private static string _clusterToken;

        public static string ClusterToken
        {
            get => _clusterToken;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _clusterToken = null;
                    return;
                }
                _clusterToken = value.Trim();
                WriteClusterTokenPath(_clusterToken, ReadGamePlatform());
            }
        }

        #endregion

        #region 读写当前游戏平台、客户端、服务器路径、ClusterToken
        /// <summary>
        /// 读取当前游戏平台[WeGame,Steam] 
        /// </summary>
        public static string ReadGamePlatform()
        {
            var platform = IniFileIo.IniFileReadString("DedicatedServerConfigure", "DedicatedServer", "Platform");
            return string.IsNullOrEmpty(platform) ? "Steam" : platform;
        }

        /// <summary>
        /// 保存当前游戏平台[WeGame,Steam]
        /// </summary>
        public static void WriteGamePlatform(string platform)
        {
            IniFileIo.IniFileWrite("DedicatedServerConfigure", "DedicatedServer", "Platform", platform);
        }

        /// <summary>
        /// 设置当前游戏平台[WeGame,Steam]
        /// </summary>
        public static void SetGamePlatform()
        {
            GamePlatform = ReadGamePlatform();
        }

        /// <summary>
        /// 获取当前游戏平台[WeGame,Steam]
        /// </summary>
        public static int GetGamePlatform()
        {
            switch (GamePlatform)
            {
                case "Steam":
                    return 0;
                case "WeGame":
                    return 1;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 读取客户端路径
        /// </summary>
        public static string ReadClientPath(string platform)
        {
            return IniFileIo.IniFileReadString("DedicatedServerConfigure", "DedicatedServer", platform + "_client_path");
        }

        /// <summary>
        /// 设置客户端路径
        /// </summary>
        public static void WriteClientPath(string clientPath, string platform)
        {
            IniFileIo.IniFileWrite("DedicatedServerConfigure", "DedicatedServer", platform + "_client_path", clientPath);
        }

        /// <summary>
        /// 读取服务端路径
        /// </summary>
        public static string ReadServerPath(string platform)
        {
            return IniFileIo.IniFileReadString("DedicatedServerConfigure", "DedicatedServer", platform + "_Server_path");
        }

        /// <summary>
        /// 设置服务端路径
        /// </summary>
        public static void WriteServerPath(string serverPath, string platform)
        {
            IniFileIo.IniFileWrite("DedicatedServerConfigure", "DedicatedServer", platform + "_Server_path", serverPath);
        }

        /// <summary>
        /// 读取ClusterToken
        /// </summary>
        public static string ReadClusterTokenPath(string platform)
        {
            return IniFileIo.IniFileReadString("DedicatedServerConfigure", "DedicatedServer", platform + "_ClusterToken");
        }

        /// <summary>
        /// 设置ClusterToken
        /// </summary>
        public static void WriteClusterTokenPath(string clusterToken, string platform)
        {
            IniFileIo.IniFileWrite("DedicatedServerConfigure", "DedicatedServer", platform + "_ClusterToken", clusterToken);
        }
        #endregion

    }
}
