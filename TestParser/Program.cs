﻿

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


var botClient = new TelegramBotClient("Bot token");
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
    string urlFromMonero = "https://api.hashvault.pro/v3/monero/wallet/49Suh9bksbqE8igcs6u7B42hb4zqtjyfM7TfkRL8s6a9X9oT8sCD7YoA5mRuHtSRUWXdgqXsqhuhiiUekfcMLHwgMbHam2Z/stats?chart=total&inactivityThreshold=10&order=-name&period=daily&poolType=false&workers=true";
    string urlFromGeco = "https://api.coingecko.com/api/v3/simple/price?ids=monero&vs_currencies=rub"; 

    string jsonres = await client.Response(urlFromMonero);
    string jsonformgeco = await client.Response(urlFromGeco);

    JObject HashVaultJson = JObject.Parse(jsonres);
    JObject CoinGecoJson = JObject.Parse(jsonformgeco);

        stats.comfirmedBalance = (string)HashVaultJson["revenue"]["comfirmedBalance"];
        stats.hashrate = (string)HashVaultJson["collective"]["hashRate"];
        stats.workers = (string)HashVaultJson["collectiveWorkers"][0]["activeMiners"];
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

    string fullsttring = $"Ваш Баланс: {stats.comfirmedBalance}\nХешрейт равен: {stats.hashrate}\nАктивных майнеров: {stats.workers}";
    //Баланс в рублях:{rounded}

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
