using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModLib.Scripting;
using UndertaleModTool.Windows;

namespace UndertaleModTool
{
    /// <summary>
    /// Logika interakcji dla klasy UndertaleObjectReference.xaml
    /// </summary>
    public partial class UndertaleObjectReference : UserControl
    {
        private static readonly MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private static readonly Regex camelCaseRegex = new("(?<=[a-z])([A-Z])", RegexOptions.Compiled);

        public event EventHandler<ObjectReferenceChangedEventArgs> ObjectReferenceChanged;

        public class ObjectReferenceChangedEventArgs : EventArgs
        {
            private object OldObject { get; }
            private object NewObject { get; }

            public ObjectReferenceChangedEventArgs(object oldObj, object newObj)
            {
                OldObject = oldObj;
                NewObject = newObj;
            }
        }

        public static DependencyProperty ObjectReferenceProperty =
            DependencyProperty.Register("ObjectReference", typeof(object),
                typeof(UndertaleObjectReference),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
                    {
                        var inst = sender as UndertaleObjectReference;
                        if (inst is null)
                            return;

                        if (e.NewValue is not null)
                        {
                            try
                            {
                                if (inst.Resources["contextMenu"] is not ContextMenu menu)
                                    return;

                                menu.DataContext = inst.ObjectReference;
                                inst.ObjectText.ContextMenu = menu;
                            }
                            catch { }
                        }
                        else
                            inst.ObjectText.ContextMenu = null;
                    }));

        public static DependencyProperty ObjectTypeProperty =
            DependencyProperty.Register("ObjectType", typeof(Type),
                typeof(UndertaleObjectReference));

        public static DependencyProperty CanRemoveProperty =
            DependencyProperty.Register("CanRemove", typeof(bool),
                typeof(UndertaleObjectReference),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
					
        public static DependencyProperty CanChangeProperty =
            DependencyProperty.Register("CanChange", typeof(bool),
                typeof(UndertaleObjectReference),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
				

        public static readonly DependencyProperty GameObjectProperty =
            DependencyProperty.Register("GameObject", typeof(UndertaleGameObject),
                typeof(UndertaleObjectReference),
                new PropertyMetadata(null));

        public static DependencyProperty ObjectEventTypeProperty =
            DependencyProperty.Register("ObjectEventType", typeof(EventType),
                typeof(UndertaleObjectReference),
                new FrameworkPropertyMetadata(EventType.Create,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static DependencyProperty ObjectEventSubtypeProperty =
            DependencyProperty.Register("ObjectEventSubtype", typeof(uint),
                typeof(UndertaleObjectReference),
                new FrameworkPropertyMetadata((uint) 0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty RoomProperty =
            DependencyProperty.Register("Room", typeof(UndertaleRoom),
                typeof(UndertaleObjectReference),
                new PropertyMetadata(null));

        public static readonly DependencyProperty RoomGameObjectProperty =
           DependencyProperty.Register("RoomGameObject", typeof(UndertaleRoom.GameObject),
               typeof(UndertaleObjectReference),
               new PropertyMetadata(null));

        public object ObjectReference
        {
            get { return GetValue(ObjectReferenceProperty); }
            set { SetValue(ObjectReferenceProperty, value); }
        }

        public Type ObjectType
        {
            get { return (Type)GetValue(ObjectTypeProperty); }
            set { SetValue(ObjectTypeProperty, value); }
        }

        public bool CanRemove
        {
            get { return (bool)GetValue(CanRemoveProperty); }
            set { SetValue(CanRemoveProperty, value); }
        }

        public bool CanChange
        {
            get { return (bool)GetValue(CanChangeProperty); }
            set { SetValue(CanChangeProperty, value); }
        }

        public UndertaleGameObject GameObject
        {
            get { return (UndertaleGameObject)GetValue(GameObjectProperty); }
            set { SetValue(GameObjectProperty, value); }
        }

        public EventType ObjectEventType
        {
            get { return (EventType)GetValue(ObjectEventTypeProperty); }
            set { SetValue(ObjectEventTypeProperty, value); }
        }

        public uint ObjectEventSubtype
        {
            get { return (uint)GetValue(ObjectEventSubtypeProperty); }
            set { SetValue(ObjectEventSubtypeProperty, value); }
        }

        public UndertaleRoom Room
        {
            get { return (UndertaleRoom)GetValue(RoomProperty); }
            set { SetValue(RoomProperty, value); }
        }

        public UndertaleRoom.GameObject RoomGameObject
        {
            get { return (UndertaleRoom.GameObject)GetValue(RoomGameObjectProperty); }
            set { SetValue(RoomGameObjectProperty, value); }
        }

        public bool IsPreCreate { get; set; } = false;

        public UndertaleObjectReference()
        {
            InitializeComponent();
            Loaded += UndertaleObjectReference_Loaded;
        }
        private void UndertaleObjectReference_Loaded(object sender, RoutedEventArgs e)
        {
            if (ObjectType is null)
                return;

            var label = TryFindResource("emptyReferenceLabel") as Label;
            if (label is null)
                return;

            string typeName = GetLocalizedTypeName(ObjectType);

            if (CanChange)
                label.Content = $"(拖放{typeName}到此处)";
            else
                label.Content = $"(空的{typeName}引用)";
        }

        private static string GetLocalizedTypeName(Type type)
        {
            switch (type.Name)
            {
                case "UndertaleSprite": return "精灵";
                case "UndertaleBackground": return "背景";
                case "UndertaleCode": return "代码";
                case "UndertaleCodeLocals": return "代码局部变量";
                case "UndertaleGameObject": return "游戏对象";
                case "UndertaleRoom": return "房间";
                case "UndertalePath": return "路径";
                case "UndertaleScript": return "脚本";
                case "UndertaleShader": return "着色器";
                case "UndertaleFont": return "字体";
                case "UndertaleTimeline": return "时间轴";
                case "UndertaleSound": return "声音";
                case "UndertaleAudioGroup": return "音频组";
                case "UndertaleEmbeddedAudio": return "内嵌音频";
                case "UndertaleEmbeddedTexture": return "内嵌纹理";
                case "UndertaleEmbeddedImage": return "内嵌图像";
                case "UndertaleTexturePageItem": return "纹理页项";
                case "UndertaleTextureGroupInfo": return "纹理组信息";
                case "UndertaleString": return "字符串";
                case "UndertaleExtension": return "扩展";
                case "UndertaleExtensionFile": return "扩展文件";
                case "UndertaleExtensionFunction": return "扩展函数";
                case "UndertaleSequence": return "序列";
                case "UndertaleAnimationCurve": return "动画曲线";
                case "UndertaleParticleSystem": return "粒子系统";
                case "UndertaleParticleSystemEmitter": return "粒子系统发射器";
                case "UndertaleFunction": return "函数";
                case "UndertaleVariable": return "变量";
            }

            string typeName = type.ToString();
            if (typeName.StartsWith("UndertaleModLib.Models.Undertale"))
            {
                // "UndertaleAudioGroup" -> "audio group"
                typeName = typeName["UndertaleModLib.Models.Undertale".Length..];
                typeName = camelCaseRegex.Replace(typeName, " $1").ToLowerInvariant();
            }
            return typeName;
        }

        public void ClearRemoveClickHandler()
        {
            RemoveButton.Click -= Remove_Click;
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (ObjectReference is null)
            {
                object oldObj = ObjectReference;
                int oldCodeCount = mainWindow.Data.Code?.Count ?? -1;

                if (GameObject is not null)
                {
                    ObjectReference = GameObject.EventHandlerFor(ObjectEventType, ObjectEventSubtype, mainWindow.Data);
                }
                else if (Room is not null)
                {
                    if (RoomGameObject is null)
                    {
                        // Generate base name
                        string name = $"gml_Room_{Room.Name.Content}_Create";

                        // If code already exists, use it (otherwise create new code)
                        if (mainWindow.Data.Code.ByName(name) is UndertaleCode existing)
                        {
                            mainWindow.ShowWarning("该房间的代码条目已存在，将复用它。");
                            ObjectReference = existing;
                        }
                        else
                        {
                            ObjectReference = UndertaleCode.CreateEmptyEntry(mainWindow.Data, name);
                        }
                    }
                    else
                    {
                        // Generate base name
                        string beginning = $"gml_RoomCC_{Room.Name.Content}_{RoomGameObject.InstanceID}";
                        string suffix = !IsPreCreate ? "_Create" : "_PreCreate";
                        string name = beginning + suffix;

                        // Ensure no duplicate names (in case instance IDs change)
                        int i = 0;
                        while (mainWindow.Data.Code.ByName(name) is not null)
                        {
                            name = beginning + "_" + (i++).ToString() + suffix;
                        }

                        ObjectReference = UndertaleCode.CreateEmptyEntry(mainWindow.Data, name);
                    }
                }
                else
                {
                    mainWindow.ShowError("当前情形下不支持添加。");
                }

                if (oldObj != ObjectReference)
                {
                    ObjectReferenceChanged?.Invoke(this, new ObjectReferenceChangedEventArgs(oldObj, ObjectReference));

                    // If a new code entry was added, mark it as new for the project system as well
                    if (mainWindow.Project is not null && oldCodeCount != -1 && 
                        oldCodeCount == mainWindow.Data.Code.Count - 1 && ObjectReference is UndertaleCode newCode)
                    {
                        mainWindow.Project.MarkAssetForExport(newCode);
                    }
                }
            }
            else
            {
                mainWindow.ChangeSelection(ObjectReference);
            }
        }

        private void Details_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ObjectReference is null)
                return;

            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
                mainWindow.ChangeSelection(ObjectReference, true);
        }
        private void OpenInNewTabItem_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ChangeSelection(ObjectReference, true);
        }
        private void MenuItem_ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            var menu = sender as ContextMenu;
            foreach (var item in menu.Items)
            {
                var menuItem = item as MenuItem;
                if ((menuItem.Header as string) == "查找所有引用")
                {
                    Type objType = menu.DataContext.GetType();
                    menuItem.Visibility = UndertaleResourceReferenceMap.IsTypeReferenceable(objType)
                                          ? Visibility.Visible : Visibility.Collapsed;

                    break;
                }
            }
        }
        private void FindAllReferencesItem_Click(object sender, RoutedEventArgs e)
        {
            var obj = (sender as FrameworkElement)?.DataContext;
            if (obj is not UndertaleResource res)
            {
                mainWindow.ShowError("所选对象不是 \"UndertaleResource\"。");
                return;
            }

            FindReferencesTypesDialog dialog = null;
            try
            {
                dialog = new(res, mainWindow.Data);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                mainWindow.ShowError("对象引用相关窗口中发生错误。\n" +
                                     $"请在 GitHub 上报告此问题。\n\n{ex}");
            }
            finally
            {
                dialog?.Close();
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ObjectReferenceChanged?.Invoke(this, new ObjectReferenceChangedEventArgs(ObjectReference, null));
            ObjectReference = null;
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ObjectReference != null)
            {
                mainWindow.ChangeSelection(ObjectReference);
            }
        }

        private void TextBox_DragOver(object sender, DragEventArgs e)
        {
            UndertaleObject sourceItem = e.Data.GetData(e.Data.GetFormats()[0]) as UndertaleObject;

            e.Effects = e.AllowedEffects.HasFlag(DragDropEffects.Link) && sourceItem != null && CanChange && sourceItem.GetType() == ObjectType ? DragDropEffects.Link : DragDropEffects.None;
            e.Handled = true;
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            UndertaleObject sourceItem = e.Data.GetData(e.Data.GetFormats()[0]) as UndertaleObject;

            e.Effects = e.AllowedEffects.HasFlag(DragDropEffects.Link) && sourceItem != null && CanChange && sourceItem.GetType() == ObjectType ? DragDropEffects.Link : DragDropEffects.None;
            if (e.Effects == DragDropEffects.Link)
            {
                ObjectReferenceChanged?.Invoke(this, new ObjectReferenceChangedEventArgs(ObjectReference, sourceItem));
                ObjectReference = sourceItem;
            }
            e.Handled = true;
        }
    }
}
