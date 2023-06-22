using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

public partial class UpdateHandler
{
    private readonly Dictionary<long, int> locationRequestMessage = new Dictionary<long, int>();
    public async Task HandleMessageUpdateAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var username = message.From?.Username
            ?? message.From.FirstName;
        logger.LogInformation("Recieved message from {username}", username);

        if (locationRequestMessage.ContainsKey(message.Chat.Id))
        {
            await RemoveMessageAsync(botClient, message.Chat.Id, locationRequestMessage[message.Chat.Id]);
            locationRequestMessage.Remove(message.Chat.Id);
        }

        if (message.Text == "/start")
        {

            var sendMessage = await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Manzilni ulashing",
                replyMarkup: new ReplyKeyboardMarkup(new KeyboardButton("Manzilni ulashing")
                {
                    RequestLocation = true
                }),
                cancellationToken: cancellationToken
            );

            locationRequestMessage.TryAdd(sendMessage.Chat.Id, sendMessage.MessageId);
            await RemoveMessageAsync(botClient, message.Chat.Id, message.MessageId);

            return; // handled /start message so return
        }

        if (message.Location is not null) // location sent
        {
            try
            {
                var weatherText = await weatherService.GetWeatherTextAsync(
                    message.Location.Latitude,
                    message.Location.Longitude,
                    cancellationToken);
                
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: weatherText,
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Xatolik yuz berdi. Support jamoasiga bog'laning!",
                    cancellationToken: cancellationToken
                );

                await HandlePollingErrorAsync(botClient, ex, cancellationToken);
            }
        }
    }

    private async Task RemoveMessageAsync(
        ITelegramBotClient botClient,
        long chatId,
        int messageId,
        TimeSpan timeSpan = default,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(timeSpan, cancellationToken);
        await botClient.DeleteMessageAsync(chatId, messageId, cancellationToken);
    }
}