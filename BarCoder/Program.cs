﻿using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using HashidsNet;
using Microsoft.Extensions.Primitives;

// Encrypt and decrypt specific kind of barcode
//     Input must be in: 
//
// ABC123-999
//
// ABC = name of the product in 3 lettrs
// 123 = Price of product (x digits)
// 999 = ID (x digits)
    
    
using var cts = new CancellationTokenSource();
TelegramBotClient client = new TelegramBotClient("", cancellationToken:cts.Token );
Hashids hasher = new Hashids();
client.SetMyCommands(new BotCommand[]
{
    new BotCommand()
    {
        // encryption input ' [XYZ100-HASH] '  [N,P-ID]
        Command = "/encrypt",
        Description = "Encrypt your sel3a "
    },
    new BotCommand()
    {
        // decryption code ' a = 0, b = 1 ... | price = input - 111 | hashId
        Command = "/decrypt",
        Description = "Decrypt your sel3a  "
    }
});
client.OnMessage += OnMessage;
Console.ReadLine();
cts.Cancel();

async Task OnMessage(Message message, UpdateType type)
{
   if (message.Text is null) return;
   if (message.Text == "/encrypt")
   {
       await client.SendMessage(message.Chat, $"Encryption input: ");
   }

   if (message.Text == "/decrypt")
   {
       await client.SendMessage(message.Chat, $"Decryption input: ");
   }

   if (message.ReplyToMessage.Text.Contains("Encrypt"))
   {
       var lower = message.Text.ToLower();
       StringBuilder sb = new StringBuilder();
       int n;
       bool isId = false;
       var digits = lower.Substring(3, lower.IndexOf('-') - 3);
       var digitsN = int.Parse(digits) * 70;
       for (int i = 0; i < 3; i++) sb.Append(((int)lower[i] - 97).ToString());
       sb.Append(digitsN);
       sb.Append('-');
       sb.Append(hasher.Encode(int.Parse(lower.Split('-')[1])));
       await client.SendMessage(message.Chat, $"{sb.ToString()}");
   }
   if (message.ReplyToMessage.Text.Contains("Decrypt"))
   {
       var lower = message.Text.ToLower();
       StringBuilder sb = new StringBuilder();
       for (int i = 0; i < 3; i++) sb.Append((char)(int.Parse(lower[i].ToString())+ 97));
       var digits = lower.Substring(3, lower.IndexOf('-') - 3);
       var digitsN = int.Parse(digits) / 70;
        sb.Append(digitsN);
        sb.Append('-');
        sb.Append((hasher.Decode(lower.Split('-')[1])[0].ToString()));
       await client.SendMessage(message.Chat, $"{sb.ToString()}");
   }

    Console.WriteLine($"Received {type} '{message.Text}' in {message.Chat}");
    // let's echo back received text in the chat
}
