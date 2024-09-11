using StreamingCourses_Domain.Entries;

namespace StreamingCourses_Implementations.Templates.Models
{
    public class AddGroupData
    {
        /// <summary>
        /// Название группы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Тип группы (ВИП или кураторы)
        /// </summary>
        public string? Type { get; set; } = default;
        /// <summary>
        /// Пригласительная ссылка
        /// </summary>
        public string? InviteLink { get; set; } = default;

        public GroupTypeEnum GetEnumType()
        {
            if (Type.Trim().ToLower().StartsWith("к"))
                return GroupTypeEnum.Curator;
            if (Type.Trim().ToLower().StartsWith("в"))
                return GroupTypeEnum.VIP;
            return GroupTypeEnum.None;
        }
    }
}
