

using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TestParser;

var botClient = new TelegramBotClient("6278666352:AAFG-St3nHyM7_e595lBqTRDGhSHbqAnlpQ");
using CancellationTokenSource cts = new();


ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() 
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();


cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;
 
    if (message.Text is not { } messageText)
        return;
    Stats stats = new();
    CoinGeco coinGeco = new();

    //httpClient
        TestParser.HttpClient client = new();
    string urlFromMonero = "https://monerohash.com/api/stats_address?address=49Suh9bksbqE8igcs6u7B42hb4zqtjyfM7TfkRL8s6a9X9oT8sCD7YoA5mRuHtSRUWXdgqXsqhuhiiUekfcMLHwgMbHam2Z&longpoll=true";
    string urlFromGeco = "https://api.coingecko.com/api/v3/simple/price?ids=monero&vs_currencies=rub"; 

    var jsonres= client.Response(urlFromMonero);
    var jsonformgeco = client.Response(urlFromGeco);

        JObject MoneroHashJson = JObject.Parse(jsonres);
        JObject CoinGecoJson = JObject.Parse(jsonformgeco);

        stats.balance = (string)MoneroHashJson["stats"]["balance"];
        stats.hashrate = (string)MoneroHashJson["stats"]["hashrate"];
        coinGeco.coin = (string)CoinGecoJson["monero"]["rub"];
        

   
    var chatId = message.Chat.Id;
   
    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
    if (message.Text == "/start")
    {
        Message sent = await botClient.SendTextMessageAsync
        (
        chatId: chatId,
        messageText = "Бот начал работу"
        );
    }
    ReplyKeyboardMarkup replyKeyboardMarkup =
    new(new[]
                {
                        new KeyboardButton[] { "Сколько?" },
                })
    {
        ResizeKeyboard = true
    };
    Percent percent = new Percent();
    string fullPriceReplace = coinGeco.coin.Replace(".", ",");
    double fullPrice = Convert.ToDouble(fullPriceReplace);
    double balance = Convert.ToDouble(stats.balance) / 1000000000000;
    double rub_balance = percent.Persent(fullPrice, balance);
    string fullsttring = $"Ваш Баланс: {balance}\nХешрейт равен:{stats.hashrate}\nБаланс в рублях:{rub_balance}";
    
    if (message.Text == "Сколько?")
    {
        {
            Message sent = await botClient.SendTextMessageAsync
        (
        chatId: chatId,
        text: fullsttring ,
        replyMarkup: replyKeyboardMarkup,
        cancellationToken: cancellationToken
        );
        }

    }
   
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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