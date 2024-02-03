using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using YourNamespace;

namespace WhiteboardApp
{
    public partial class MainWindow : Window
    {
        private Polyline currentLine;  // 当前绘制的线条
        private bool isDrawing;        // 是否正在绘制
        private bool usingEraser;      // 是否使用橡皮擦
        private Brush currentColor;    // 当前选定的颜色
        private int penThickness = 2;  // 笔刷粗细
        private int eraserThickness = 20; // 橡皮擦粗细
        private List<UIElement> currentPageElements = new List<UIElement>(); // 当前页的所有元素
        private List<List<UIElement>> allPagesElements = new List<List<UIElement>>(); // 所有页的所有元素
        private int currentPageIndex = 0;   // 当前页的索引

        public MainWindow()
        {
            InitializeComponent();
            currentColor = Brushes.Black;
            allPagesElements.Add(currentPageElements);
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
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                currentLine.Points.Add(e.GetPosition(drawingCanvas));
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
            Button button = (Button)sender;
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
                Color selectedColor = colorDialog.SelectedColor; // 确保这里是 System.Windows.Media.Color

                // 使用选中的颜色创建一个 SolidColorBrush
                SolidColorBrush colorBrush = new SolidColorBrush(selectedColor);

                // 更新当前颜色，此处假设有一个currentColor变量且其类型为 SolidColorBrush
                currentColor = colorBrush; // 现在 currentColor 应该是 SolidColorBrush 类型的变量

                // 其他逻辑，比如关闭橡皮擦模式
                usingEraser = false;
            }
        }


    }
}
