using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotChat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static string token = "1186610447:AAFLQrjCIqqSKqrtbDbSOkM60tGa-E33k7w";
        Telegram.Bot.TelegramBotClient bot;
        Thread thb;
        InlineKeyboardMarkup mainmenue;
        ReplyKeyboardMarkup selectname;
        ChatBotEntities botdb = new ChatBotEntities();
        private void Form1_Load(object sender, EventArgs e)
        {
            thb = new Thread(ChatBot);
            thb.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thb.Abort();
        }

        void ChatBot()
        {
            bot = new Telegram.Bot.TelegramBotClient(token);
            int offset = 0;

            while (true)
            {

            var update = bot.GetUpdatesAsync(offset).Result;
                foreach (var up in update)
                {

                    offset = up.Id + 1;

                    if(up.CallbackQuery != null)
                    {
                        var callback = up.CallbackQuery.Data;
                        var chatidcall = up.CallbackQuery.Message.Chat.Id;
                        if (callback.Contains("startBot"))
                        {
                            var user = botdb.users.SingleOrDefault(u => u.chatid == up.Id);
                            if(user == null)
                            {
                                user = new user()
                                {
                                    chatid = up.CallbackQuery.Message.Chat.Id,
                                    username = up.CallbackQuery.Message.Chat.Username
                                };
                            }

                            string message = "✨ نام خود را وارد کنید ✨";
                            bot.SendTextMessageAsync(chatidcall, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, getMenuName());
                        }
                    }


                    if (up.Message == null)
                        continue;
                    var chatid = up.Message.Chat.Id;
                    var txt = up.Message.Text;
                    var from = up.Message.From;

                    if (txt.ToLower().Contains("/start"))
                    {
                        string message = @"
🍯 سلام به ربات چت ناشناس ما خوش آمدید 💋
--------------------------------------
🍯 برای شروع کار خود و ثبت نام در این ربات دکمه ی شروع را کلیک کنید 🥂
--------------------------------------";
                        bot.SendTextMessageAsync(chatid, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, GetMenuStart());
                    }
                    else if (txt.Contains("بعد از وارد کردن اسم کلیک کنید"))
                    {
                        string message = @"";
                        bot.SendTextMessageAsync(chatid, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, GetMenuStart());
                    }
                }  
            }
        }
        InlineKeyboardMarkup GetMenuStart()
        {
            InlineKeyboardButton[][] row1 = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("📨 شروع چت  📨","startBot"),
                }
            };
            mainmenue = new InlineKeyboardMarkup(row1);
            return mainmenue;
        }
        ReplyKeyboardMarkup getMenuName()
        {
            KeyboardButton[][] row1 = new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("📮 بعد از وارد کردن اسم کلیک کنید 📮")
                }
            };
            return selectname = new ReplyKeyboardMarkup(row1);
        }
    }
}
