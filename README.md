# StreamingCoursesBot
Используемые технологии:
1. TelegramBot Api
2. _IRON_PROGRAMMER_BOT_ConsoleApp_ использует _Polling_
3. _IronProgrammerBotWebApplication_ использует _Webhooks_
4. Хранение состояния пользователя реализовано через _NoSql БД FireBase_
5. Данные: Роли, Пользователи, Курсы, Студенты, Группы и т.д. хранятся в _MSSQL БД_
6. Разработка баз данных через _CodeFirst_ и миграций _EntityFrameworkCore_
7. 

Перед началом работы необходимо:
1. Создать проект в [Firebases](https://console.firebase.google.com/).

2. прописать следующие данные в appsettings:
   - Firebase - BasePath и Secret
   - ConnectionStrings - строку подключения к БД
   - BotConfiguration - настройки бота

4. В файле добавить в коде данные администратора

**По умолчанию есть 4 роли:**
1. Администратор
2. Куратор
3. Студент
4. Пользователь
   
**Функции администратора:**
1. Создавать (загружать данные по шаблону):
   - Курсы
   - Кураторов
   - Телеграм группы
   - Студентов
2. Связывать сущности Курс - Группа - Куратор - Студент
3. Запускать новый курс
4. Выгружать статистику по студенту и куратору в каждом курсе

**Для запуска курса администратору необходимо:**
1. Заполнить все необходимые данные:
   "Создать курс":
   Из главного меню:
   - "Создать курс"
   - "Скачать шаблон"
   - "Назад"
   - "Загрузить данные"
   - вложить excel файл с заполненными данными
      - При успешной валидации курс создается
      - Если возникнут ошибки валидации, то они будут указаны с номером строки
   - Аналогично созданию курса скачать и заполнить шаблоны для:
      - "Создать группы"
      - "Создать кураторов"
      - "Создать студентов"
2. Добавить загруженные данные к курсу:
   - Из главного меню выбрать "Мои курсы" и выбрать нужный курс
   - Добавить группы к курсу и выбрать нужные из ранее загруженного документа по шаблону
   - Аналогично группам добавить студентов и кураторов
4. Для каждого выбранного куратора нужно проставить кол-во мест:
5. В выбранном курсе выбрать:
   - "Список кураторов"
   - Нажать на фамилию куратора
   - "Заполнить кол-во мест к куратору"
   - Выбрать нужное кол-во и подтвердить
6. Вернуться к выбранному курсу и нажать "Запустить курс". Если все верно сделано, то бужет сообщение "Курс запущен", иначе выведется ошибка валидации. После этого студент может выбрать куратора на курс
  
   - [Видеодемонстрация функций администратора. Часть 1](https://disk.yandex.ru/i/ydb0TV_EgBU_1g)
   - [Видеодемонстрация функций администратора. Часть 2](https://disk.yandex.ru/i/dUDubTPz7CQY0g)
   - [Видеодемонстрация функций администратора. Часть 3](https://disk.yandex.ru/i/Ce4Jo_fTPI85Ew)

**Функции куратора**
1. "Мои курсы":
   - открывается список курсов:
     - "Мои студенты":
       - Выбранный студент => страница с контактными данными
2. "Мой профиль":
   - "Изменить профиль":
        - Скачать шаблон, заполнить его и загрузить документ

- [Видеодемонстрация функций куратора](https://disk.yandex.ru/i/9UA6nkS8q4_y1w)

**Функции студента:**
1. "Мои курсы"
   - "Написать в чат группы" (если администратор добавл группы к курсу отображается ссылка по тарифу _ВИП_ или _Кураторы_)
   - Выбранный курс:
     - "Заполнить личные данные" (студент не может выбрать куратора, пока не указал необходимые данные: Фамилия, Имя, электронная почта, ник на Гитхабе):
       - После подтверждения можно перейти сразу в главное меню или обрвтно к выбранному курсу
   - "Выбрать куратора" (после запуска курса администратором у студента появляется эта кнопка)
     - Отображается список доступных кураторов (если у кого-то из кураторов уже нет мест для записи, его не в списке для выбора)
       => Выбрать куратора по Фамилии => "Выбрать куратора" => "Подтвердить" => Отображается сообщение с результатом записи
       После успешной записи куратору приходит сообщение от бота с информацией о студенте и курсе
   - "Написать куратору" => переход в личные сообщения

 - [Видеодемонстрация функций студента - заполнение личных данных](https://disk.yandex.ru/i/KQTYqiwSn5BmkQ)
 - [Видеодемонстрация функций студента - выбор куратора](https://disk.yandex.ru/i/v78lKZqjQsE-EQ)
 
**Функции пользователя**:
Для всех остальных пользователей отображается стандартная приветственная страница с контактами школы [Iron Programmer](https://ironprogrammer.ru/) 

В видеоматериалах использованы данные кураторов с их согласия:
- Сергей Борщев [@GudzON28](https://github.com/GudzON28)
- Алексей Миронов [@algmironov](https://github.com/algmironov)
- Николай Смирнов [@Nikolay200](https://github.com/Nikolay200)
- Кирилл Фисенко [@KirillFisenko](https://github.com/KirillFisenko)

