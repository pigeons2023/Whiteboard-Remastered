using System.Threading.Tasks;
using System.Windows;

namespace WhiteboardApp
{
    public partial class SplashScreenWindow : Window
    {
        public SplashScreenWindow()
        {
            InitializeComponent();
            ShowSplashScreen();
        }

        private async void ShowSplashScreen()
        {
            // 使用更小的增量和更短的延迟来更新进度条，以实现平滑效果
            for (int progress = 0; progress < 100; progress += 2)
            {
                splashProgressBar.Value = progress;
                await Task.Delay(30);
            }

            // 确保进度条完全填满
            splashProgressBar.Value = 100;
            await Task.Delay(3400); // 添加一个延迟


            // 启动页面显示完毕后，打开主窗口
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            // 关闭启动页面
            this.Close();
        }

    }
}
