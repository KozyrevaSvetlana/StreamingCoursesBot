﻿using StreamingCourses_Domain.Helpers;
using StreamingCourses_Contracts;
using StreamingCourses_Contracts.Abstractions;
using StreamingCourses_Implementations.Helpers;
using StreamingCourses_Implementations.Pages.Base;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StreamingCourses_Implementations.Pages.Admin
{
    public class AddStudentsPage(IServiceProvider services) : CallbackQueryPageBase(services)
    {
        public override async Task<string?> GetText(UserState userState)
        {
            return await Task.FromResult(string.Format(Resourses.Pages.StudentsList));
        }

        public override async Task<ButtonLinqPage[][]> GetKeyboard(UserState userState)
        {
            var studentService = services.GetService<IStudentService>();
            var students = await studentService!.GetStudentsWithoutCourseIdAsync(userState.UserData!.CourseData!.Id);

            var keyboard = new List<List<ButtonLinqPage>>();

            foreach (var student in students)
            {
                var fio = $"{student.User?.LastName} {student.User?.FirstName} - Тариф: {student.Group?.Type?.GetDescription()}".Trim();
                var text = (userState.UserData.SelectedIds?.Contains(student.Id.ToString())) ?? false ?
                    "✅ " + fio : fio;

                keyboard.Add(
                    new() { new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(text, student.Id.ToString())) });
            }

            keyboard.Add(
                new() { new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Confirm)) });

            keyboard.Add(
                new() { new ButtonLinqPage(InlineKeyboardButton.WithCallbackData(ButtonConstants.Back), services.GetService<BackwardDummyPage>()) });

            return keyboard.Select(k => k.ToArray()).ToArray();
        }
        public override async Task<UserState> ProcessHandle(UserState userState, Update? update = null)
        {
            userState.UserData.ResultText = string.Empty;
            if (update?.CallbackQuery?.Data == ButtonConstants.Back)
            {
                userState.UserData.SelectedIds = null;
                return userState;
            }
            if (update?.CallbackQuery?.Data == ButtonConstants.Confirm)
            {
                var studentService = services.GetService<IStudentService>();
                var ids = userState.UserData.SelectedIds!.Select(id => Int32.Parse(id)).ToArray();
                var isSaved = await studentService!.AddStudentsToCourseAsync(ids, userState.UserData.CourseData!.Id);
                userState.Pages.Pop();
                if (isSaved)
                {
                    userState.UserData.SelectedIds = null;
                    userState.UserData.ResultText = "Данные успешно сохранены!";
                    userState.AddPage(new ResultPage(services));
                    return userState;
                }

                userState.UserData.ResultText = "При сохранении данных произошла ошибка! Попробуйте позднее";
                userState.AddPage(new ResultPage(services));
                return userState;
            }

            if (Int32.TryParse(update?.CallbackQuery?.Data, out var id) && id > 0)
            {
                var selectedId = update!.CallbackQuery!.Data;
                userState.UserData.SelectedIds ??= new();
                if (userState.UserData.SelectedIds.Contains(selectedId))
                {
                    userState.UserData.SelectedIds.Remove(selectedId);
                }
                else
                {
                    userState.UserData.SelectedIds.Add(selectedId);
                }
            }
            return userState;
        }
    }
}