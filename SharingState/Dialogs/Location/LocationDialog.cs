using Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SharingState.Dialogs
{
    public class LocationDialog : ComponentDialog
    {
        public LocationDialog() : base(nameof(LocationDialog))
        {
            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                 DisplayLocationFromComposer
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> DisplayLocationFromComposer(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("(reading data back your postcode from the Composer/Adaptive state variable");

            var state = stepContext.GetState();
            var userScope = state.GetValue<object>("user");

            string postcode = JObject.Parse(userScope.ToString())["postcode"].ToString();

            await stepContext.Context.SendActivityAsync("OK, so your postcode is " + postcode);
            await stepContext.Context.SendActivityAsync("Do any of these match your address?:");
            // invoke http postcode lookup and present choices:
            WebRequest request = WebRequest.Create("https://api.postcodes.io/postcodes/" + postcode);
            WebResponse webResponse = request.GetResponse();

            using (Stream dataStream = webResponse.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();

                // Display the content.  
                await stepContext.Context.SendActivityAsync("I've extracted the following data from postcodes.io for the postcode that you supplied in the Composer/Adaptive Dialog:");
                await stepContext.Context.SendActivityAsync(responseFromServer);
            }

            return await stepContext.ContinueDialogAsync(cancellationToken);
        }
    }
}
