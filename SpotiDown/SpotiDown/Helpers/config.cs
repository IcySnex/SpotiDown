namespace SpotiDown.Helpers
{
    public class config
    {
        public config_metadata metadata { get; set; } = new config_metadata();
        public config_preferences prefernces { get; set; } = new config_preferences();
        public config_auth oauth { get; set; } = new config_auth();
    }

    public class config_metadata
    {
        public bool lyrics { get; set; } = true;
        public bool artwork { get; set; } = true;
    }
    public class config_preferences
    {
        public int quality { get; set; } = 3;
        public int format { get; set; } = 0;
        public string downloadpath { get; set; } = "/SpotiDown";
        public string filename { get; set; } = "/{artist}/{title}";
    }
    public class config_auth
    {
        public string id { get; set; } = "bd6035158be648b695451636f2e311f2";
        public string secret { get; set; } = "0dd522b08dc646559e5fc7ef092fa9d7";
    }
}
