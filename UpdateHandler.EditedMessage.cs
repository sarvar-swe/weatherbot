using Telegram.Bot;
using Telegram.Bot.Types;

public partial class UpdateHandler
{
    public Task HandleEditedMessageUpdateAsync(ITelegramBotClient botClient, Message editedMessage, CancellationToken cancellationToken)
    {
        var username = editedMessage.From?.Username
            ?? editedMessage.From.FirstName;
        logger.LogInformation("Recieved editedMessage from {username}", username);
        return Task.CompletedTask;
    }
}