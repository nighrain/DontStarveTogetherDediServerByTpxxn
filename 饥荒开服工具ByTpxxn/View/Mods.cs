﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using 饥荒开服工具ByTpxxn.Class.DedicateServer;
using 饥荒开服工具ByTpxxn.Class.Tools;
using 饥荒开服工具ByTpxxn.MyUserControl;
using Color = System.Windows.Media.Color;

namespace 饥荒开服工具ByTpxxn.View
{
    public partial class DedicatedServerPage : Page
    {
        /// <summary>
        /// 设置 "Mod集"
        /// </summary>
        private void SetModSet()
        {   // 设置
            if (!string.IsNullOrEmpty(CommonPath.ServerModsDirPath))
            {
                // 清空,Enabled变成默认值
                foreach (var item in _mods.ModList)
                {
                    item.Enabled = false;
                }
                // 细节也要变成默认值,之后再重新读取
                foreach (var mod in _mods.ModList)
                {
                    foreach (var configurationOption in mod.ConfigurationOptions)
                    {
                        configurationOption.Value.Current = configurationOption.Value.Default;
                    }
                }
                // 重新读取
                _mods.ReadModsOverrides(_dediFilePath.ModConfigOverworldFilePath);
            }
            // 显示 
            ModListStackPanel.Children.Clear();
            ModSettingStackPanel.Children.Clear();
            ModDescriptionStackPanel.Text = "";
            if (_mods != null)
            {
                for (var i = 0; i < _mods.ModList.Count; i++)
                {
                    var dediModBox = new DediModBox
                    {
                        Width = 250,
                        Height = 100,
                        ContentMod = _mods.ModList[i],
                        UCCheckBox = { Tag = i },
                    };
                    dediModBox.UCCheckBox.IsChecked = _mods.ModList[i].Enabled;
                    dediModBox.UCCheckBox.Checked += CheckBox_Checked;
                    dediModBox.UCCheckBox.Unchecked += CheckBox_Unchecked;
                    dediModBox.PreviewMouseLeftButtonDown += DediModBox_MouseLeftButtonDown;
                    ModListStackPanel.Children.Add(dediModBox);
                }
            }
        }

        /// <summary>
        /// 设置 "Mod" "MouseLeftButtonDown"
        /// </summary>
        private void DediModBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // [Mod信息]
            var n = (int)((DediModBox)sender).UCCheckBox.Tag;
            ModInfoImage.Source = _mods.ModList[n].Picture != null ? PictureHelper.ChangeBitmapToImageSource(_mods.ModList[n].Picture) : null;
            ModInfoNameTextBlock.Text = _mods.ModList[n].Name;
            ModInfoAuthorTextBlock.Text = "作者：" + _mods.ModList[n].Author;
            ModInfoVersionTextBlock.Text = "版本：" + _mods.ModList[n].Version;
            ModInfoFolderTextBlock.Text = "文件夹：" + _mods.ModList[n].ModDirName;
            ModInfoTypeTextBlock.Text = _mods.ModList[n].ModType == ModType.Server ? "服务器" : "所有人";
            // [Mod描述]
            ModDescriptionStackPanel.Text =_mods.ModList[n].Description;
            // [Mod设置]
            if (_mods.ModList[n].ConfigurationOptions.Count == 0)
            {
                // 没有细节配置项
                Debug.WriteLine(n);
                ModSettingStackPanel.Children.Clear();
            }
            else
            {
                // 有,显示细节配置项
                Debug.WriteLine(n);
                ModSettingStackPanel.Children.Clear();
                foreach (var item in _mods.ModList[n].ConfigurationOptions)
                {
                    // stackPanel
                    var stackPanel = new StackPanel
                    {
                        Height = 40,
                        Width = 330,
                        Orientation = Orientation.Horizontal
                    };
                    var modOptionTitleTextBlock = new TextBlock()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 150,
                        Foreground = new SolidColorBrush(Color.FromRgb(197, 170, 115)),
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        FontWeight = FontWeights.Bold,
                        Text = string.IsNullOrEmpty(item.Value.Label) ? item.Value.Name : item.Value.Label
                    };
                    // dediComboBox
                    var dediSelectBox = new DediSelectBox
                    {
                        Height = stackPanel.Height,
                        Width = 150,
                        Foreground = new SolidColorBrush(Color.FromRgb(197, 170, 115)),
                        FontSize = 12,
                        Tag = n + "$" + item.Key,
                        TextList = item.Value.Options.Select(option => option.Description).ToList()
                    };
                    // 把当前选择mod的第n个,放到tag里
                    dediSelectBox.TextIndex = CurrentDescriptionToTextIndex(item.Value.CurrentDescription, dediSelectBox.TextList);
                    dediSelectBox.SelectionChangedWithSender += DediComboBox_SelectionChanged;
                    // 添加
                    stackPanel.Children.Add(modOptionTitleTextBlock);
                    stackPanel.Children.Add(dediSelectBox);
                    ModSettingStackPanel.Children.Add(stackPanel);
                }
                UpdateLayout();
            }
        }

        private int CurrentDescriptionToTextIndex(string currentDescription, List<string> dediSelectBoxTextList)
        {
            for (var i = 0; i < dediSelectBoxTextList.Count; i++)
            {
                if (currentDescription == dediSelectBoxTextList[i])
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// 设置 "Mod" "SelectionChanged"
        /// </summary>
        private void DediComboBox_SelectionChanged(object sender)
        {
            Debug.WriteLine(((DediSelectBox)sender).Tag);
            var str = ((DediSelectBox)sender).Tag.ToString().Split('$');
            if (str.Length != 0)
            {
                var n = int.Parse(str[0]);
                var name = str[1];
                // 好复杂
                _mods.ModList[n].ConfigurationOptions[name].Current =
                    _mods.ModList[n].ConfigurationOptions[name].Options[((DediSelectBox)sender).TextIndex].Data;

            }
        }

        /// <summary>
        /// 设置 "Mod" "CheckBox_Unchecked"
        /// </summary>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _mods.ModList[(int)((CheckBox)sender).Tag].Enabled = false;
            //Debug.WriteLine(((CheckBox)sender).Tag.ToString());
        }

        /// <summary>
        /// 设置 "Mod" "CheckBox_Checked"
        /// </summary>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _mods.ModList[(int)((CheckBox)sender).Tag].Enabled = true;
            //Debug.WriteLine(((CheckBox)sender).Tag.ToString());
        }
    }
}
