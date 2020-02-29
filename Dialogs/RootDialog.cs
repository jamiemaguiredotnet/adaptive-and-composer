// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Debugging;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Recognizers.Text;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{

    public class RootDialog : ComponentDialog
    {
        protected readonly Bot.Builder.BotState _userState;
        
        public RootDialog(UserState userState) : base("root")
        {
            _userState = userState;
       
            // Get Folder of dialogs.
            var resourceExplorer = new ResourceExplorer().AddFolder(@"C:\Users\Jamie\Downloads\blog-post-adaptive-and-composer\Dialogs");

            // find the main composer dialog to start with
            var composerDialog = resourceExplorer.GetResource("Main.dialog");
            
            // hyrdate an Adaptive Dialogue
            AdaptiveDialog myComposerDialog = DeclarativeTypeLoader.Load<AdaptiveDialog>(composerDialog, resourceExplorer, DebugSupport.SourceMap);
            myComposerDialog.Id = "Main.dialog";
            
            // setup lanaguage generation for the dialogue
            myComposerDialog.Generator = new TemplateEngineLanguageGenerator(new TemplateEngine().AddFile(@"C:\Users\Jamie\Downloads\blog-post-adaptive-and-composer\Dialogs\ComposerDialogs\Main\Main.lg"));

            // add to the ComponentDialog which Root dialogue inherits from
            AddDialog(myComposerDialog);

            // create a waterfall dialogue and begin our adaptive dialogue
            AddDialog(new WaterfallDialog("waterfall", new WaterfallStep[] { BeginComposerAdaptiveDialog }));

        }

        private async Task<DialogTurnResult> BeginComposerAdaptiveDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _userState.SaveChangesAsync(stepContext.Context, false, cancellationToken);

            return await stepContext.BeginDialogAsync("Main.dialog", null, cancellationToken);
        }

    }
}
