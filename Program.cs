using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("5303353761:AAHlIvNZ94_lE_tud_fs-iY5LDTQT3VH4Vk");


int blockLevel = 0;
bool messDeleted = false;
string[] badWords = new string[] { "bad word", "badword" };
string[] veryBadWords = new string[] { "very bad word", "verybadword" };


int year;
int month;
int day;
int hour;
int minute;
int second;


long chatId = 0;
string messageText;
int messageId;
string firstName;
string lastName;
long id;
Message sentMessage;
int pollId = 0;

//----------------------//

year = int.Parse(DateTime.UtcNow.Year.ToString());
month = int.Parse(DateTime.UtcNow.Month.ToString());
day = int.Parse(DateTime.UtcNow.Day.ToString());
hour = int.Parse(DateTime.UtcNow.Hour.ToString());
minute = int.Parse(DateTime.UtcNow.Minute.ToString());
second = int.Parse(DateTime.UtcNow.Second.ToString());
Console.WriteLine("Data: " + year + "/" + month + "/" + day);
Console.WriteLine("Time: " + hour + ":" + minute + ":" + second);


using var cts = new CancellationTokenSource();


var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { } 
};
botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();


Console.WriteLine($"\nHello! I'm {me.Username} and i'm your Bot!");


Console.ReadKey();
cts.Cancel();

//----------------------//


async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Type != UpdateType.Message)
        return;
    if (update.Message!.Type != MessageType.Text)
        return;

    chatId = update.Message.Chat.Id;
    messageText = update.Message.Text;
    messageId = update.Message.MessageId;
    firstName = update.Message.From.FirstName;
    lastName = update.Message.From.LastName;
    id = update.Message.From.Id;
    year = update.Message.Date.Year;
    month = update.Message.Date.Month;
    day = update.Message.Date.Day;
    hour = update.Message.Date.Hour;
    minute = update.Message.Date.Minute;
    second = update.Message.Date.Second;

    Console.WriteLine("\nData message --> " + year + "/" + month + "/" + day + " - " + hour + ":" + minute + ":" + second);

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId} from user:\n" + firstName + " - " + lastName + ".");

    messageText = messageText.ToLower();


        if (messageText == "/vulgarity")
        {
            switch (blockLevel)
            {
                case 0:
                    blockLevel = 1;
                    await botClient.SendTextMessageAsync
                    (
                    chatId: chatId,
                    text: "Vulgarity: \"Medium block\".",
                     cancellationToken: cancellationToken
                    );
                    return;

                case 1:
                    blockLevel = 2;
                    await botClient.SendTextMessageAsync
                    (
                    chatId: chatId,
                    text: "Vulgarity: \"Hard block\".",
                     cancellationToken: cancellationToken
                    );
                    return;
                case 2:
                    blockLevel = 0;
                    await botClient.SendTextMessageAsync
                    (
                    chatId: chatId,
                    text: "Vulgarity: \"Block disabled\".",
                     cancellationToken: cancellationToken
                    );
                    return;
            }
        }

        for (int x = 0; x < badWords.Length; x++)
        {

            if (messageText.Contains(badWords[x]) && blockLevel == 2 && !messDeleted)
            {
                messDeleted = true;
                await botClient.DeleteMessageAsync(chatId, messageId);
                sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: firstName + " " + lastName + " you can't say that things",
            //replyMarkup: replyKeyboardMarkup,
            cancellationToken: cancellationToken);
            }
        }
        for (int x = 0; x < veryBadWords.Length; x++)
        {
            if (messageText.Contains(veryBadWords[x]) && (blockLevel == 1 || blockLevel == 2) && !messDeleted)
            {
                messDeleted = true;
                await botClient.DeleteMessageAsync(chatId, messageId);
                sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: firstName + " " + lastName + " you can't say that things",
            //replyMarkup: replyKeyboardMarkup,
            cancellationToken: cancellationToken);
            }
        }
        messDeleted = false;

        if (messageText == "hello")
        {
            sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Hello, " + firstName + " " + lastName + "!",
            //replyMarkup: replyKeyboardMarkup,
            cancellationToken: cancellationToken);
        }

        if (messageText == "meme")
        {
            sentMessage = await botClient.SendPhotoAsync(
            chatId: chatId,
            photo: "https://i.redd.it/uhkj4abc96r61.jpg",
            caption: "<b>MEME</b>",
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
        }

        if (messageText == "sound")
        {
            Message message = await botClient.SendAudioAsync(
             chatId: chatId,
             audio: "",
             cancellationToken: cancellationToken);
        }

        if (messageText == "Video")
        {
            Message message = await botClient.SendVideoAsync(
            chatId: chatId,
            video: "",
            thumb: "",
            supportsStreaming: true,
            cancellationToken: cancellationToken);
        }

        if (messageText == "album")
        {
            Message[] messages = await botClient.SendMediaGroupAsync(
            chatId: chatId,
            media: new IAlbumInputMedia[]
            {
                new InputMediaPhoto(""),
                new InputMediaPhoto(""),
            },
            cancellationToken: cancellationToken);
        }

        if (messageText == "doc")
        {
            //Use sendDocument method to send general files.
            Message message = await botClient.SendDocumentAsync(
            chatId: chatId,
            document: "",
            caption: "",
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
        }

        if (messageText == "gif")
        {
            //Use sendAnimation method to send animation files(GIF or H.264 / MPEG - 4 AVC video without sound).
            Message message = await botClient.SendAnimationAsync(
            chatId: chatId,
            animation: "",
            caption: "",
            cancellationToken: cancellationToken);
        }


        if (messageText == "poll")
        {
            pollId = messageId + 1;

            Console.WriteLine($"\nPoll number: {pollId}!");
            Message pollMessage = await botClient.SendPollAsync(
            chatId: chatId,
            question: "How are you?",
            options: new[]
            {
                "Good!",
                "I could be better.."
            },
            cancellationToken: cancellationToken);
        }

        if (messageText == "close poll")
        {
            Console.WriteLine($"\nPoll number {pollId} is close!");
            Poll poll = await botClient.StopPollAsync(
            chatId: chatId,
            messageId: pollId,
            cancellationToken: cancellationToken);
        }


        if (messageText == "send me the phone number of anna")
        {
            Message message = await botClient.SendContactAsync(
            chatId: chatId,
            phoneNumber: "+1234567890",
            firstName: "Anna",
            lastName: "",
            cancellationToken: cancellationToken);
        }

        if (messageText == "roma location")
        {
            Message message = await botClient.SendVenueAsync(
                chatId: chatId,
                latitude: 41.9027835f,
                longitude: 12.4963655,
                title: "Rome",
                address: "Italy, Rome",
                cancellationToken: cancellationToken);
        }

        if (messageText == "send me a location")
        {
            Message message = await botClient.SendLocationAsync(
                chatId: chatId,
                latitude: 56.3476781f,
                longitude: 53.1257704,
                cancellationToken: cancellationToken);
        }
    }


Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
