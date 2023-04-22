

using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TestParser;

var botClient = new TelegramBotClient("6010175081:AAHFtfLWHXpXcG_clKCdmdOcAKjcatLGQKI");
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
    try
    {
        //httpClient
        HttpClient httpClient = new();
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://monerohash.com/api/stats_address?address=49Suh9bksbqE8igcs6u7B42hb4zqtjyfM7TfkRL8s6a9X9oT8sCD7YoA5mRuHtSRUWXdgqXsqhuhiiUekfcMLHwgMbHam2Z&longpoll=true");
        using HttpResponseMessage response = await httpClient.SendAsync(request);
        //Проверка статуса сайта
        Console.WriteLine(response.StatusCode);

        
        Console.WriteLine("\nContent");
        string content = await response.Content.ReadAsStringAsync();
        JObject jObj = JObject.Parse(content);
        stats.balance = (string)jObj["stats"]["balance"];
        stats.hashrate = (string)jObj["stats"]["hashrate"];
        
        Console.WriteLine(content);

    }
    catch (Exception ex) { }
    float num =(float)Convert.ToDouble(stats.balance);
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
    string line = "-------------------------------------";
 
    ReplyKeyboardMarkup replyKeyboardMarkup =
    new(new[]
                {
                        new KeyboardButton[] { "Сколько?" },
                })
    {
        ResizeKeyboard = true
    };
    float full = 0.000000000000f;
    float sum = num+full;
    
    string fullsttring = $"Ваш Баланс: {sum}\nХешрейт равен:{stats.hashrate}";
    
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