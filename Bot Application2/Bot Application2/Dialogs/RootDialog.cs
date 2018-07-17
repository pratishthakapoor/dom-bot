using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Bot_Application2.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            if(activity.Text.Contains("Hi") || activity.Text.Contains("hi"))
            {
                //await context.PostAsync("Hi i am a FAQ bot ");
               
                /*
                 * Prompt dialog used to prompt a message to the user from the bot side.
                 * */

                PromptDialog.Text(
                    context,
                    callFaqService,
                    "Hi, I am DFAQ a Domino's FAQ bot. So, let's get started",
                    "Oops, some error occurred.Sorry for the inconvinence");
                
            }

            //context.Wait(MessageReceivedAsync);
        }

        private async Task callFaqService(IDialogContext context, IAwaitable<string> result)
        {
            var userAnswer = await result;

            //connection to the qnaMaker api which contains the Knowledge base in the form of question and answer format

            // use try and catch block

            try
            {
                //subscription key for the QnA maker kb stored in the web config file.

                var subscriptionkey = ConfigurationManager.AppSettings["QnASubscriptionKey"];

                /*
                 * knowledge base id used to connect to the kb in the qnamaker details can be found in the 
                 * http call of qnamaker website
                 **/
                
                var knowledgeBaseId = ConfigurationManager.AppSettings["QnAKnowledgeBaseId"];

                // call the GetQnaResponse method of QnAMakerDialog

                var query = new QnAMakerDialog.QnAMakerDialog().GetQnAResponse(userAnswer, subscriptionkey, knowledgeBaseId);

                var response = query.ans.FirstOrDefault();

                if (response != null && response.score >= 50.0)
                    await context.PostAsync(response.ans);
            }
            
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}