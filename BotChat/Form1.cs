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
        //counstructor
        public Form1()
        {
            InitializeComponent();
        }

        //token bot
        static string token = "1186610447:AAFLQrjCIqqSKqrtbDbSOkM60tGa-E33k7w";
        static string constring = "";

        static void DataBaseCheck()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder conect = new System.Data.SqlClient.SqlConnectionStringBuilder();
            conect.DataSource = "localhost";
            conect.InitialCatalog = "chatBot";
            conect.IntegratedSecurity = true;
            constring = conect.ConnectionString;
            ChatBotDataContext test = new ChatBotDataContext(constring);
            if (test.DatabaseExists())
                return;
            test.CreateDatabase();
        }
        ChatBotDataContext botdb = new ChatBotDataContext(constring);

        //my bot
        Telegram.Bot.TelegramBotClient bot;
        //tread app
        Thread thb;
        //data base model

        #region markup menu buttons
        InlineKeyboardMarkup mainmenue;
        InlineKeyboardMarkup selectgener;
        InlineKeyboardMarkup selectState;
        //ReplyKeyboardMarkup ;
        #endregion


        //load app and start thread
        #region loding form
        private void Form1_Load(object sender, EventArgs e)
        {
            //check  exist data base
            if (!botdb.Database.Exists())
            {
                //creat data base
                botdb.Database.Create();
            }
            thb = new Thread(ChatBot);
            thb.Start();
            DataBaseCheck();
        }
        #endregion

        //closeing app and close thread
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thb.Abort();
        }

        void ChatBot()
        {
            bot = new Telegram.Bot.TelegramBotClient(token);
            //set offset for update message or ... bot
            int offset = 0;


            //start app by while
            #region Code My Bot ->
            while (true)
            {
                //get update
                var update = bot.GetUpdatesAsync(offset).Result;

                //foreach for chekc all message & ...
                foreach (var up in update)
                {
                    offset = up.Id + 1;
                    //inline btn callback
                    #region inline button and check callback
                    //check call back
                    if (up.CallbackQuery != null)
                    {
                        //get call ack data
                        var callback = up.CallbackQuery.Data;
                        //get chat id - user
                        var chatidcall = up.CallbackQuery.Message.Chat.Id;
                        if (callback.Contains("startBot"))
                        {
                            //check exist user in data base
                            var user = botdb.users.SingleOrDefault(u => u.chatid == up.CallbackQuery.From.Id);
                            if (user == null)
                            {
                                //creat user
                                user = new user()
                                {
                                    chatid = up.CallbackQuery.Message.Chat.Id,
                                    username = up.CallbackQuery.Message.Chat.Username
                                };
                                //add user in data base
                                botdb.users.Add(user);
                                botdb.SaveChanges();
                            }
                            string message = @"✅ جنسیت خود را وارد کنید ✅";
                            bot.SendTextMessageAsync(chatidcall, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, getButtosInlineGener());
                        }
                        //if gener user = boy
                        else if (callback.Contains("boy"))
                        {
                            var user = botdb.users.SingleOrDefault(u => u.chatid == up.CallbackQuery.Message.Chat.Id);
                            if (user != null)
                            {
                                //edit user section gener
                                user.gener = "پسر";
                                botdb.SaveChanges();
                                string message = @"ذخیره شد 📬
 شهر خود را وارد کنید 🔍
📌  توجه در صورتی که شهر خود را یافت نکردید به پشتیبانی جهت افزودن شهر در ربات اطلاع دهید! 👨🏻‍💻";
                                bot.SendTextMessageAsync(chatidcall, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, GetMenuState());
                            }
                        }
                        //if gener user = girl//if gener user = girl
                        else if (callback.Contains("girl"))
                        {
                            //edit user section gener
                            var user = botdb.users.SingleOrDefault(u => u.chatid == up.CallbackQuery.Message.Chat.Id);
                            if (user != null)
                            {
                                //edit user section gener
                                user.gener = "دختر";
                                botdb.SaveChanges();
                                string message = @"ذخیره شد 📬
 شهر خود را وارد کنید 🔍
📌  توجه در صورتی که شهر خود را یافت نکردید به پشتیبانی جهت افزودن شهر در ربات اطلاع دهید! 👨🏻‍💻";
                                bot.SendTextMessageAsync(chatidcall, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0,GetMenuState());
                            }
                        }
                            //edit user section city
                        else if (botdb.States.SingleOrDefault(s => s.Name == callback) != null)
                        {
                            //edit user section gener
                            var user = botdb.users.SingleOrDefault(u => u.chatid == up.CallbackQuery.Message.Chat.Id);
                            if (user != null)
                            {
                                //edit user section city
                                user.city = callback;
                                botdb.SaveChanges();
                                string message = @"ذخیره شد 📬";
                                bot.SendTextMessageAsync(chatidcall, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0);
                            }
                        }
                    }
                    #endregion

                    //if null message
                    if (up.Message == null)
                        continue;

                    //answer btn keyboard 
                    var chatid = up.Message.Chat.Id;
                    var txt = up.Message.Text;
                    var from = up.Message.From;
                    //if click btn /start
                    #region check message request keyboard button
                    if (txt.ToLower().Contains("/start"))
                    {
                        string message = @"
🍯 سلام به ربات چت ناشناس ما خوش آمدید 💋
--------------------------------------
🍯 برای شروع کار خود و ثبت نام در این ربات دکمه ی شروع را کلیک کنید 🥂
--------------------------------------";
                        bot.SendTextMessageAsync(chatid, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, GetMenuStart());
                    }

                    //if click btn ok name 
                    //else if (txt.Contains("بعد از وارد کردن اسم کلیک کنید"))
                    //{
                    //    string message = @"";
                    //    bot.SendTextMessageAsync(chatid, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, GetMenuStart());
                    //}
                    //else if (txt.Contains("نامم را وارد کردم"))
                    //{
                    //    user.name = name;
                    //    string message = @"";
                    //    bot.SendTextMessageAsync(chatid, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0);
                    //}

                    #endregion
                }
            }
            #endregion

        }

        #region Function Buttons
        //function get button for start bot
        InlineKeyboardMarkup GetMenuStart()
        {
            //creat keyboard nutton
            InlineKeyboardButton[][] start = new InlineKeyboardButton[][]
            {
                //creat first button in row 1
                new InlineKeyboardButton[] {InlineKeyboardButton.WithCallbackData("📨 شروع چت  📨","startBot"), }
            };
            //set keyboard button in markup and return
            mainmenue = new InlineKeyboardMarkup(start);
            return mainmenue;
        }
        InlineKeyboardMarkup getButtosInlineGener()
        {
            //creat keyboard nutton
            InlineKeyboardButton[][] gener = new InlineKeyboardButton[][]
            {
                //creat first button in row 1
                new InlineKeyboardButton[] {InlineKeyboardButton.WithCallbackData(" 🙋🏻‍♂️ پسرم ", "boy"), InlineKeyboardButton.WithCallbackData("🙋🏻‍♀️ دخترم ", "girl") }
            };
            //set keyboard button in markup and return
            selectgener = new InlineKeyboardMarkup(gener);
            return selectgener;
        }
        InlineKeyboardMarkup GetMenuState()
        {
            //creat keyboard nutton
            InlineKeyboardButton[][] arrayState = new InlineKeyboardButton[botdb.States.Count()][];
            int i = 0;
            foreach (var state in botdb.States)
            {
                var btnstate = botdb.States.SingleOrDefault(s => s.Id == (i+1));
                //creat first button in row 1
               var btn = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(btnstate.Name,btnstate.Name) };
                arrayState[i] = btn;
                i++;
            }
            //set keyboard button in markup and return
            selectState = new InlineKeyboardMarkup(arrayState);
            return selectState;
        }
        //function get button for enter name
        //ReplyKeyboardMarkup getButtonsGener()
        //{
        //    //creat keyboard button
        //    KeyboardButton[][] allButtons = new KeyboardButton[][]
        //    {
        //        //creat first button in row 1
        //        new KeyboardButton[]{new KeyboardButton() },
        //        new KeyboardButton[]{new KeyboardButton() }
        //    };
        //    //set button in markup and return
        //    return selectgener = new ReplyKeyboardMarkup(allButtons);
        //}
        #endregion
    }
}
