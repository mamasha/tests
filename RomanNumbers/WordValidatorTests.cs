using System.Linq.Expressions;

class WordValidatorTests
{
    private static void assert(Expression<Func<bool>> expr)
    {
        try
        {
            if (expr.Compile()())
                return;

            Console.WriteLine($"Assertion failed in {expr}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in in {expr}; {ex}");
        }
    }

    public static void RunAll()
    {
        IRuleProcessor rules = new RuleProcessor(cfg => {
            cfg.Chars.Add('a', new RuleProcessor.CharConfig { Followers = { 'a', 'b', 'd' }, IsFinal = true });
            cfg.Chars.Add('b', new RuleProcessor.CharConfig { Followers = { 'a', 'f' } });
            cfg.Chars.Add('c', new RuleProcessor.CharConfig { Followers = { 'a' }, IsFinal = true });
        });

        IWordValidator validator = new WordValidator(rules);

        assert(() => validator.IsValid("ak") == false);
        assert(() => validator.IsValid("ab") == false);
        assert(() => validator.IsValid("aba") == true);
    }
}