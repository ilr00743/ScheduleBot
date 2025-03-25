using System.Collections;
using Telegram.Bot.Types.ReplyMarkups;

namespace PIBScheduleBot;

public class MarkupDrawer
{
    public ReplyKeyboardMarkup DrawMainMenu()
    {
        var mainMenu = new ReplyKeyboardMarkup(new []
        {
            new KeyboardButton[] { new("\ud83d\udccb Розклад на сьогодні"), new("\ud83d\udccb Розклад на наступний день") },
            new KeyboardButton[] { new("\u26a0\ufe0f Зміни на сьогодні"), new("\u26a0\ufe0f Зміни на наступний день") },
            new KeyboardButton[] { new("#\ufe0f\u20e3 Пошук за групою"), new("\ud83e\uddd1\u200d\ud83c\udfeb Пошук за викладачем"), new("\ud83d\udcc5 Пошук за днем тижня") }
        });
    
        mainMenu.ResizeKeyboard = true;
        
        return mainMenu;
    }

    public ReplyKeyboardMarkup DrawStatusSettings()
    {
        var markup = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { new("Студент"), new("Викладач") }
        });
        
        markup.ResizeKeyboard = true;
        
        return markup;
    }

    public ReplyKeyboardMarkup DrawMarkupWithSize<T>(int buttonsPerRow)
    {
        var entities = new List<int>{1,2,3,4,5,6,7,8,9,10,11};
        var markup = new ReplyKeyboardMarkup { ResizeKeyboard = true };

        for (int i = 0; i < entities.Count; i += buttonsPerRow)
        {
            var row = entities.Skip(i).Take(buttonsPerRow)
                .Select(entity => new KeyboardButton(entity.ToString()))
                .ToArray();
            
            markup.AddNewRow(row);
        }

        markup.AddNewRow(new KeyboardButton("Головне меню"));

        return markup;
    }
}