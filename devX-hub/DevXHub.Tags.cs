namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        public interface ITagValue
        {
            string Value { get; }
        }

        public interface ITag : ITagValue
        {
            string Name { get; }
        }

        public class Tag<T> : ITag
            where T: ITagValue
        {
            private readonly string _value;

            public Tag(string value)
            {
                _value = value;
            }

            string ITag.Name => typeof(T).Name;
            string ITagValue.Value => _value;
        }
    }

    public class SandboxTag : DevXHub.Tag<SandboxTag>
    {
        public SandboxTag(string sandboxId)
            : base(sandboxId)
        {}
    }
}
