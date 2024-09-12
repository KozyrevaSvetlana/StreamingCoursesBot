# StreamingCoursesBot
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

Для того, чтобы администратору запустить курс необходимо:
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
6. Вернуться к выбранному курсу и нажать "Запустить курс". Если все верно сделано, то бужет сообщение "Курс запущен", иначе выведется ошибка валидации
