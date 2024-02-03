using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using YourNamespace;
using System.Drawing; // 用于 Icon 类
using System;
using System.IO; // 用于 MemoryStream 类
using System.Windows.Forms; // 用于 NotifyIcon

namespace WhiteboardApp
{
    public partial class MainWindow : Window
    {
        private Polyline currentLine;  // 当前绘制的线条
        private bool isDrawing;        // 是否正在绘制
        private bool usingEraser;      // 是否使用橡皮擦
        private System.Windows.Media.Brush currentColor;    // 当前选定的颜色
        private int penThickness = 2;  // 笔刷粗细
        private int eraserThickness = 20; // 橡皮擦粗细
        private List<UIElement> currentPageElements = new List<UIElement>(); // 当前页的所有元素
        private List<List<UIElement>> allPagesElements = new List<List<UIElement>>(); // 所有页的所有元素
        private int currentPageIndex = 0;   // 当前页的索引
        private NotifyIcon notifyIcon = null; // 系统托盘图标

        public MainWindow()
        {
            InitializeComponent();
            currentColor = System.Windows.Media.Brushes.Black;
            allPagesElements.Add(currentPageElements);

            InitializeNotifyIcon(); // 初始化 NotifyIcon

            // 设置为无边框窗口
            this.WindowStyle = WindowStyle.None;

            // 设置窗口为最大化状态，以覆盖整个屏幕
            this.WindowState = WindowState.Maximized;

            // 监听窗口状态改变事件
            this.StateChanged += MainWindow_StateChanged;
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon();

            // 使用 Base64 字符串设置图标
            string base64Icon = "AAABAAEAFBQAAAEAIAC4BgAAFgAAACgAAAAUAAAAKAAAAAEAIAAAAAAAQAYAAMEOAADBDgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAnfv7vJ3///yd///8mfv6/AAAAAAAAAAAmfv7fAAAAACZ+/t8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACd///8AAAAAAAAAAAAAAAAAAAAAJn7+vyd///8nf///J3///yZ+/r8AAAAAAAAAAAAAAAAAAAD/AAAA/wAAAP8AAAD/AAAAAAAAAAAAAAAAJ3///wAAAAAAAAAAAAAAAAAAAP8AAAD/J3///wAAAAAnf///AAAAAAAAAAAAAAAAAAAAAAAAAP8AAAAAAAAAAAAAAAAAAAD/AAAA/wAAAP8nf///AAAA/wAAAP8AAAD/AAAA/x1fv/8nf///J3///yd///8mfv6/AAAAAAAAAAAAAAAAAAAA/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACd+/s8nf///J3///yZ+/r8AAAAAAAAA/yZ+/tsAAAAAJn7+nwAAAAAAAAAAAAAAAAAAAAAAAAD/AAAAAAAAAP8AAAAAAAAAAAAAAP8AAAAAAAAA/wAAAP8AAAD/AAAA/wAAAAAAAAD/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP8AAAAAAAAA/wAAAAAAAAAAAAAA/wAAAAAAAAD/AAAAAAAAAAAAAAD/AAAAAAAAAP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA/wAAAAAAAAD/AAAAAAAAAAAAAAD/AAAAAAAAAP8AAAAAAAAAAAAAAP8AAAAAAAAA/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP8AAAD/AAAAAAAAAP8AAAD/AAAA/wAAAP8AAAAAAAAA/wAAAP8AAAD/AAAA/wAAAAAAAAD/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA/wAAAAAAAAAAAAAA/wAAAAAAAAAAAAAA/wAAAAAAAAD/AAAAAAAAAAAAAAD/AAAAAAAAAP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD/AAAAAAAAAAAAAAD/AAAA/wAAAP8AAAD/AAAAAAAAAP8AAAD/AAAA/wAAAP8AAAAAAAAA/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA/wAAAP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA/wAAAP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA/wAAAAAAAAAAAAAA/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD/AAAA/wAAAP8AAAD/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA///wAP/DUAD/3gAA4dxQAO4AAADvwlAA60LwAOta8ADrWvAAyELwANta8ADYQvAA3/7wAM/88ADAAfAA/t/wAP4f8AD///AA///wAP//8AA=";
            notifyIcon.Icon = Base64StringToIcon(base64Icon);

            notifyIcon.Visible = true; // 初始设置为可见

            // 创建上下文菜单（右键菜单）
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem menuItemClose = new ToolStripMenuItem("关闭");

            // 添加菜单项到上下文菜单
            contextMenu.Items.Add(menuItemClose);
            notifyIcon.ContextMenuStrip = contextMenu;

            // 为关闭菜单项添加事件处理
            menuItemClose.Click += (sender, e) => CloseApplication();

            notifyIcon.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                    notifyIcon.Visible = false;
                };
        }


        public static Icon Base64StringToIcon(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return new Icon(ms);
            }
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
                notifyIcon.Visible = true;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
            }
            base.OnClosing(e);
        }


        // 鼠标按下事件
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentLine = new Polyline
                {
                    // 如果使用橡皮擦，则将笔刷设置为背景色（相当于擦除），否则使用当前选定的颜色
                    Stroke = usingEraser ? drawingCanvas.Background : currentColor,
                    StrokeThickness = usingEraser ? eraserThickness : penThickness
                };
                drawingCanvas.Children.Add(currentLine);
                currentLine.Points.Add(e.GetPosition(drawingCanvas));
                isDrawing = true;
            }
        }

        // 鼠标移动事件
        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isDrawing)
            {
                System.Windows.Point position = e.GetPosition(drawingCanvas);

                // 假设 toolBarHeight 是工具栏的高度
                double toolBarHeight = 5; // 您需要根据实际情况来获取或设置这个值

                // 确保鼠标位置在画布的有效区域内，且低于工具栏的底部
                if (position.X >= 0 && position.X <= drawingCanvas.ActualWidth &&
                    position.Y >= toolBarHeight && position.Y <= drawingCanvas.ActualHeight)
                {
                    currentLine.Points.Add(position);
                }
            }
        }



        // 鼠标释放事件
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
            currentPageElements.Add(currentLine);
            currentLine = null;
        }

        // 颜色按钮点击事件
        private void Color_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            currentColor = button.Background;
            usingEraser = false;
        }

        // 橡皮擦按钮点击事件
        private void Eraser_Click(object sender, RoutedEventArgs e)
        {
            usingEraser = true;
        }

        // 笔刷粗细滑块值变化事件
        private void sliderPenThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            penThickness = (int)e.NewValue;
        }

        // 橡皮擦粗细滑块值变化事件
        private void sliderEraserThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            eraserThickness = (int)e.NewValue;
        }

        // 上一页按钮点击事件
        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex > 0)
            {
                currentPageIndex--;
                drawingCanvas.Children.Clear();
                currentPageElements = allPagesElements[currentPageIndex];
                foreach (UIElement elem in currentPageElements)
                {
                    drawingCanvas.Children.Add(elem);
                }
            }
        }

        // 下一页按钮点击事件
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageIndex < allPagesElements.Count - 1)
            {
                currentPageIndex++;
                drawingCanvas.Children.Clear();
                currentPageElements = allPagesElements[currentPageIndex];
                foreach (UIElement elem in currentPageElements)
                {
                    drawingCanvas.Children.Add(elem);
                }
            }
            else
            {
                currentPageElements = new List<UIElement>();
                allPagesElements.Add(currentPageElements);
                currentPageIndex++;
            }
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog(); // 显示关于页面为对话框
        }
        private void Palette_Click(object sender, RoutedEventArgs e)
        {
            // 创建并显示自定义颜色选择对话框
            ColorPickerDialog colorDialog = new ColorPickerDialog();
            if (colorDialog.ShowDialog() == true)
            {
                // 获取用户选择的颜色
                System.Windows.Media.Color selectedColor = colorDialog.SelectedColor; // 确保这里是 System.Windows.Media.Color

                // 使用选中的颜色创建一个 SolidColorBrush
                SolidColorBrush colorBrush = new SolidColorBrush(selectedColor);

                // 更新当前颜色，此处假设有一个currentColor变量且其类型为 SolidColorBrush
                currentColor = colorBrush; // 现在 currentColor 应该是 SolidColorBrush 类型的变量

                // 其他逻辑，比如关闭橡皮擦模式
                usingEraser = false;
            }
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            // 最小化窗口
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            // 定义一个计数器，用于记录用户回答“Yes”的次数
            int yesCount = 0;

            // 第一次询问
            if (System.Windows.MessageBox.Show("您要退出白板吗？", "退出", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                yesCount++;
            }

            // 第二次询问
            if (yesCount == 1 && System.Windows.MessageBox.Show("请再确认一下，您是否要退出白板？", "退出", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                yesCount++;
            }

            // 第三次询问
            if (yesCount == 2 && System.Windows.MessageBox.Show("亲，不退出白板行吗？", "退出", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                // 仅在用户前两次选择“Yes”且第三次选择“No”后关闭窗口
                this.Close();
            }
        }
        private void CloseApplication()
        {
            // 确认是否真的要关闭应用程序
            if (System.Windows.MessageBox.Show("您要退出白板吗？", "退出", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (notifyIcon != null)
                {
                    notifyIcon.Dispose();
                }
                this.Close();
            }
        }
    }
}