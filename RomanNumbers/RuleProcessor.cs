interface IRuleProcessor
{
    bool CanBeFinal(char ch);
    bool FollowingOrder(char ch, char follower);
}

class RuleProcessor : IRuleProcessor
{
    public class CharConfig
    {
        public bool IsFinal { get; set; }
        public HashSet<char> Followers { get; set; } = new();
    }

    public class Config
    {
        public Dictionary<char, CharConfig> Chars { get; set; } = new();
    }

    private readonly Config _config;

    public RuleProcessor(Action<Config> setConfig)
    {
        var config = new Config();
        setConfig(config);

        _config = config;
    }

    private CharConfig GetCharConfig(char ch)
    {
        _config.Chars.TryGetValue(ch, out var charConfig);
        return charConfig;
    }

    bool IRuleProcessor.CanBeFinal(char ch)
    {
        var charConfig = GetCharConfig(ch);

        if (charConfig == null)
            return false;

        return
            charConfig.IsFinal;
    }

    bool IRuleProcessor.FollowingOrder(char ch, char follower)
    {
        var charConfig = GetCharConfig(ch);

        return
            charConfig is not null &&
            charConfig.Followers.Contains(follower);
    }
}
