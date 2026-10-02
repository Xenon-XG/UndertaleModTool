using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.Windows
{
    /// <summary>
    /// Interaction logic for FindReferencesTypesDialog.xaml
    /// </summary>
    public partial class FindReferencesTypesDialog : Window
    {
        private static readonly MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

        private readonly UndertaleResource sourceObj;
        private readonly UndertaleData data;
        private readonly bool dontShowWindow = false;

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible || IsLoaded)
                return;

            if (dontShowWindow)
            {
                Close();
                return;
            }

            if (Settings.Instance.EnableDarkMode)
                MainWindow.SetDarkTitleBarForWindow(this, true, false);
        }

        public FindReferencesTypesDialog(UndertaleResource obj, UndertaleData data)
        {
            InitializeComponent();

            if (data.GeneralInfo is null)
            {
                this.ShowError("无法确定 GameMaker 版本——“常规信息”为空。");
                dontShowWindow = true;
                return;
            }

            (Type, string)[] sourceTypes = UndertaleResourceReferenceMap.GetTypeMapForVersion(obj.GetType(), data);
            if (sourceTypes is null)
            {
                this.ShowError($"无法获取类型为“{obj.GetType()}”的对象的来源类型。");
                dontShowWindow = true;
                return;
            }

            foreach (var typePair in sourceTypes)
            {
                TypesList.Items.Add(new CheckBox()
                {
                    DataContext = typePair.Item1,
                    Content = typePair.Item2,
                    IsChecked = true
                });
            }

            sourceObj = obj;
            this.data = data;
        }
        public FindReferencesTypesDialog(UndertaleData data)
        {
            InitializeComponent();

            if (data.GeneralInfo is null)
            {
                this.ShowError("无法确定 GameMaker 版本——“常规信息”为空。");
                dontShowWindow = true;
                return;
            }

            GameVersion ver;
            if (data.GeneralInfo.Branch == UndertaleGeneralInfo.BranchType.LTS2022_0)
                ver = (2022, 0, 0);
            else
                ver = (data.GeneralInfo.Major, data.GeneralInfo.Minor, data.GeneralInfo.Release);
            var sourceTypes = UndertaleResourceReferenceMap.GetReferenceableTypes(ver);

            foreach (var typePair in sourceTypes)
            {
                if (data.Code is null && UndertaleResourceReferenceMap.CodeTypes.Contains(typePair.Key))
                    continue;

                TypesList.Items.Add(new CheckBox()
                {
                    DataContext = typePair.Key,
                    Content = typePair.Value,
                    IsChecked = true
                });
            }

            this.data = data;
        }

        public bool ShowReferencesFor(UndertaleResource sourceObj, HashSetTypesOverride typesList, bool showIfNoResults = true)
        {
            FindReferencesResults dialog = null;
            try
            {
                var results = UndertaleResourceReferenceMethodsMap.GetReferencesOfObject(sourceObj, data, typesList);
                if (results is null && !showIfNoResults)
                    return false;

                dialog = new(sourceObj, data, results);
                dialog.Show();

                return true;
            }
            catch (Exception ex)
            {
                mainWindow.ShowError("对象引用相关窗口发生错误。\n" +
                                     $"请在 GitHub 上报告此问题。\n\n{ex}");
                dialog?.Close();
                return false;
            }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in TypesList.Items)
            {
                if (item is CheckBox checkBox)
                    checkBox.IsChecked = true;
            }
        }
        private void DeselectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in TypesList.Items)
            {
                if (item is CheckBox checkBox)
                    checkBox.IsChecked = false;
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (sourceObj is not null)
            {
                HashSetTypesOverride typesList = new();
                foreach (var item in TypesList.Items)
                {
                    if (item is CheckBox checkBox && checkBox.IsChecked == true)
                    {
                        if (checkBox.DataContext is Type t)
                            typesList.Add(t);
                    }
                }

                if (typesList.Count == 0)
                {
                    this.ShowError("至少应选择一种类型。");
                    return;
                }

                ShowReferencesFor(sourceObj, typesList);
            }
            else
            {
                Dictionary<Type, string> typesDict = new();
                foreach (var item in TypesList.Items)
                {
                    if (item is CheckBox checkBox && checkBox.IsChecked == true)
                    {
                        if (checkBox.DataContext is Type t)
                            typesDict[t] = checkBox.Content as string;
                    }
                }

                if (typesDict.Count == 0)
                {
                    this.ShowError("至少应选择一种类型。");
                    return;
                }

                if (typesDict.Count > 1 && typesDict.ContainsKey(typeof(UndertaleString))
                    && data.Strings.Count > 5000)
                {
                    var res = this.ShowQuestion("字符串数量很多，而你选择了“字符串”类型。\n" +
                                                "这可能会使搜索过程明显变长。\n" +
                                                "是否继续？");
                    if (res != MessageBoxResult.Yes)
                        return;
                }

                Hide();

                FindReferencesResults dialog = null;
                try
                {
                    var results = await UndertaleResourceReferenceMethodsMap.GetUnreferencedObjects(data, typesDict);
                    dialog = new(data, results);
                    dialog.Show();
                }
                catch (Exception ex)
                {
                    mainWindow.ShowError("对象引用相关窗口发生错误。\n" +
                                         $"请在 GitHub 上报告此问题。\n\n{ex}");
                    dialog?.Close();
                }
            }

            Close();
        }
    }
}
