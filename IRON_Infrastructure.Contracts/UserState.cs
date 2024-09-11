using StreamingCourses_Contracts.Abstractions;

namespace StreamingCourses_Contracts
{
    /// <summary>
    /// Состояние пользователя
    /// </summary>
    public record class UserState(Stack<IPage> Pages, UserData UserData)
    {
        public IPage CurrentPage => Pages.Peek();

        public void AddPage(IPage page)
        {
            if (!Pages.Any() || CurrentPage.GetType() != page.GetType())
            {
                Pages.Push(page);
            }
        }

        public void GoToPage(string pageName)
        {
            while (Pages.Count > 1)
            {
                Pages.Pop();
                if (Pages.Count == 1 || CurrentPage.GetType().Name == pageName)
                    return;
            }
        }
    }
}
