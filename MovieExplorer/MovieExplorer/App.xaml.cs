namespace MovieExplorer
{
    public partial class App : Application
    {
        public static HttpClient HttpClient { get; private set; } = new HttpClient();

        public App()
        {
            InitializeComponent();
            var handler = new HttpClientHandler
            {
                //Use system proxy settings
                UseProxy = true,
                //Use default credentials
                UseDefaultCredentials = true
            };

            HttpClient = new HttpClient(handler)
            {
                //Set timeout to 60 seconds
                Timeout = TimeSpan.FromSeconds(60)
            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
        
    }
}