using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace YourNamespace
{
    public partial class ColorPickerDialog : Window
    {
        public Color SelectedColor { get; private set; }

        public ColorPickerDialog()
        {
            InitializeComponent();
            InitializeColorList();
        }

        private void InitializeColorList()
        {
            // 使用反射获取所有预定义的颜色
            var colors = typeof(Colors).GetProperties().Select(prop =>
                new { Name = prop.Name, Color = (Color)prop.GetValue(null) });

            lstColors.ItemsSource = colors;
            lstColors.DisplayMemberPath = "Name";
            lstColors.SelectedValuePath = "Color";
        }

        private void BtnCustomColor_Click(object sender, RoutedEventArgs e)
        {
            // 打开系统颜色对话框选择自定义颜色
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedColor = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                DialogResult = true;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (lstColors.SelectedValue != null)
            {
                SelectedColor = (Color)lstColors.SelectedValue;
            }
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
