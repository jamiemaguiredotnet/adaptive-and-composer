using Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

public class UserProfileDialog : ComponentDialog
{

    private IStatePropertyAccessor<UserProfile> _userStateAccessor;
   
    public UserProfileDialog(UserState userState) : base(nameof(UserProfileDialog))
    {
        _userStateAccessor = userState.CreateProperty<UserProfile>("UserProfile");

        // This array defines how the Waterfall will execute.
        var waterfallSteps = new WaterfallStep[]
        {
            GetName,
            Greet
        };

        // Add named dialogs to the DialogSet. These names are saved in the dialog state.
        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        AddDialog(new TextPrompt(nameof(TextPrompt)));

        // The initial child Dialog to run.
        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> GetName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var promptOptions = new PromptOptions
        {
            Prompt = MessageFactory.Text("Please enter your name.")
        };

        return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
    }

    private async Task<DialogTurnResult> Greet(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        await stepContext.Context.SendActivityAsync("Pleased to meet you "+ stepContext.Result.ToString());

        return await stepContext.ContinueDialogAsync(cancellationToken);
    }

}

