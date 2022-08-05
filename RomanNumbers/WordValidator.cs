using System.Diagnostics;

interface IWordValidator
{
    bool IsValid(string word);
}

class WordValidator : IWordValidator
{ 
    private readonly IRuleProcessor _rules;

    public WordValidator(IRuleProcessor rules)
    {
        _rules = rules;
    }

    bool IWordValidator.IsValid(string word)
    {
        if (string.IsNullOrEmpty(word))
            return false;

        for (var i = 0; i < word.Length - 1; i++)
        {
            if (!_rules.FollowingOrder(word[i], word[i + 1]))
                return false;
        }

        if (!_rules.CanBeFinal(word[^1]))
            return false;

        return true;
    }
}

