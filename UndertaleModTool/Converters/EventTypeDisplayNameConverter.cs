using System;
using System.Globalization;
using System.Windows.Data;
using UndertaleModLib.Models;

namespace UndertaleModTool
{
    /// <summary>
    /// 将游戏对象事件类型（索引或枚举值）转换为用于显示的中文名称。
    /// 仅用于显示；逻辑判断仍使用 <see cref="EventNameConverter"/> 输出的枚举名。
    /// </summary>
    [ValueConversion(typeof(uint), typeof(string))]
    public class EventTypeDisplayNameConverter : IValueConverter
    {
        public static string GetDisplayName(EventType type)
        {
            return type switch
            {
                EventType.Create => "创建 (Create)",
                EventType.Destroy => "销毁 (Destroy)",
                EventType.Alarm => "警报 (Alarm)",
                EventType.Step => "步 (Step)",
                EventType.Collision => "碰撞 (Collision)",
                EventType.Keyboard => "键盘 (Keyboard)",
                EventType.Mouse => "鼠标 (Mouse)",
                EventType.Other => "其他 (Other)",
                EventType.Draw => "绘制 (Draw)",
                EventType.KeyPress => "按键按下 (KeyPress)",
                EventType.KeyRelease => "按键释放 (KeyRelease)",
                EventType.Trigger => "触发器 (Trigger)",
                EventType.CleanUp => "清理 (CleanUp)",
                EventType.Gesture => "手势 (Gesture)",
                EventType.PreCreate => "预创建 (PreCreate)",
                _ => type.ToString()
            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EventType type)
                return GetDisplayName(type);

            uint val = System.Convert.ToUInt32(value);
            return GetDisplayName((EventType)val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
