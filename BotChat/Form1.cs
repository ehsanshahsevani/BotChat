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
        ReplyKeyboardMarkup selectname;
        #endregion
        

        //load app and start thread
        #region loding form
        private void Form1_Load(object sender, EventArgs e)
        {
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
                    #region inline button
                    if (up.CallbackQuery != null)
                    {
                        var callback = up.CallbackQuery.Data;
                        var chatidcall = up.CallbackQuery.Message.Chat.Id;
                        if (callback.Contains("startBot"))
                        {
                            var user = botdb.users.SingleOrDefault(u => u.chatid == up.Id);
                            if (user == null)
                            {
                                user = new user()
                                {
                                    chatid = up.CallbackQuery.Message.Chat.Id,
                                    username = up.CallbackQuery.Message.Chat.Username
                                };
                            }

                            string message = @"✨ نام خود را وارد کنید ✨
📮 بعد از وارد کردن اسم کلیک کنید 📮";
                            bot.SendTextMessageAsync(chatidcall, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, getMenuName());
                        }
                    }
                    #endregion

                    if (up.Message == null)
                        continue;

                    //answer btn keyboard 

                    var chatid = up.Message.Chat.Id;
                    var txt = up.Message.Text;
                    var from = up.Message.From;

                    //if click btn /start
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
                    else if (txt.Contains("بعد از وارد کردن اسم کلیک کنید"))
                    {
                        string message = @"";
                        bot.SendTextMessageAsync(chatid, message, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, GetMenuStart());
                    }
                }
            }
            #endregion

        }

        //function get button for start bot
        #region Function Buttons


        InlineKeyboardMarkup GetMenuStart()
        {
            //creat keyboard nutton
            InlineKeyboardButton[][] row1 = new InlineKeyboardButton[][]
            {
                //creat first button in row 1
                new InlineKeyboardButton[] {InlineKeyboardButton.WithCallbackData("📨 شروع چت  📨","startBot"), }
            };
            //set keyboard button in markup and return
            mainmenue = new InlineKeyboardMarkup(row1);
            return mainmenue;
        }

        //function get button for enter name
        ReplyKeyboardMarkup getMenuName()
        {
            //creat inline keyboard button
            KeyboardButton[][] allButtons = new KeyboardButton[][]
            {
                //creat first inline button iin row 1
                new KeyboardButton[]{new KeyboardButton("📮 وارد کردم 📮")}
            };
            //set inline button in markup and return
            return selectname = new ReplyKeyboardMarkup(allButtons);
        }
        #endregion
    }
}
