namespace MovieExplorer
{
    public partial class App : Application
    {
        public static HttpClient HttpClient { get; private set; } = new HttpClient();

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}