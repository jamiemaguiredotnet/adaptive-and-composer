using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Recognizers.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.Dialogs.Debugging;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Dialogs
{
    public class RootDialog : ComponentDialog
    {
        protected readonly Microsoft.Bot.Builder.BotState _userState;
       
        public RootDialog(UserState userState) : base("root")
        {
            _userState = userState;

            AddDialog(new UserProfileDialog(userState));

            // The initial child Dialog to run.
            InitialDialogId = "waterfall";

            // Get Folder of dialogs.
            var resourceExplorer = new ResourceExplorer().AddFolder("Dialogs");

            // find the main composer dialog to start with
            var composerDialog = resourceExplorer.GetResource("Main.dialog");

            // hyrdate an Adaptive Dialogue
            AdaptiveDialog myComposerDialog = DeclarativeTypeLoader.Load<AdaptiveDialog>(composerDialog, resourceExplorer, DebugSupport.SourceMap);
            myComposerDialog.Id = "Main.dialog";

            // setup lanaguage generation for the dialogue
            myComposerDialog.Generator = new TemplateEngineLanguageGenerator(new TemplateEngine().AddFile(@"C:\Users\Jamie\source\repos\composer-adaptive-regular-hybrid\Dialogs\ComposerDialogs\Main\Main.lg"));

            // add to the ComponentDialog which Root dialogue inherits from
            AddDialog(myComposerDialog);

            AddDialog(new WaterfallDialog("waterfall", new WaterfallStep[] { StartDialogAsync, BeginComposerAdaptiveDialog }));
        }

        private async Task<DialogTurnResult> StartDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync("UserProfileDialog", null, cancellationToken);
        }

        private async Task<DialogTurnResult> BeginComposerAdaptiveDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _userState.SaveChangesAsync(stepContext.Context, false, cancellationToken);

            return await stepContext.BeginDialogAsync("Main.dialog", null, cancellationToken);
        }
       
    }
}
