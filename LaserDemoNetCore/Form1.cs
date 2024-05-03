
namespace LaserDemoNetCore
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Thread t = new Thread(a);
            t.UnsafeStart();
        }

        private void a(object? obj)
        {
            throw new NotImplementedException();
        }
    }
}
