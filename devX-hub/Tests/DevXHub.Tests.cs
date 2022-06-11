using System.Threading.Tasks;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        public static class Tests
        {
            public static void RunAll()
            {
            }
        }
    }

    public static class DevXHubTestHelpers
    {
        public static async Task RunPocMessages(this IDevXHub devXHub, DevXHub.PocMessagesRequest request)
        {
            var sandboxId = request.sandboxId;

            var updates = new[] {
                $"{sandboxId}: Blueprints are validated ... ",
                $"{sandboxId}: Infrastructure is created ... ",
                $"{sandboxId}: Artifacts are deployed ... ",
                $"{sandboxId}: Terraform '{request.terraformName}' is initialized ...",
                $"{sandboxId}: Terraform '{request.terraformName}' is applied ...",
                $"{sandboxId}: K8S agent is started ...",
                $"{sandboxId}: Helm is applied ...",
                $"{sandboxId}: Sandbox is ready"
            };

            devXHub.Log.Info("Running POC messages", request);

            await Task.Delay(request.delayOnStart.Seconds());

            for (var i = 0; i < updates.Length; i++)
            {
                await Task.Delay(request.delayOnMessage.Seconds());

                var msg = updates[i];
                var no = updates.Length - i;
                await devXHub.Notify(sandboxId, msg, new { no });
            }
        }
    }
}
