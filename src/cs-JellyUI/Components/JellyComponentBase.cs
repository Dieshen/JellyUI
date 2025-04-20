using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Jelly.UI
{
    public abstract class JellyComponentBase : ComponentBase
    {
        private ILogger? _logger;

        #region Dependency Injections
        [Inject]
        private ILoggerFactory LoggerFactory { get; set; } = default!;
        #endregion

        #region Parameters
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; } = default!;

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Parameter]
        public string? Style { get; set; }
        #endregion

        public ElementReference Element { get; set; }
        protected bool IsRenderComplete { get; private set; }
        protected ILogger Logger => _logger ??= LoggerFactory.CreateLogger(GetType());

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            IsRenderComplete = true;
            return Task.CompletedTask;
        }
    }
}
