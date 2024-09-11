using System.ComponentModel;

namespace StreamingCourses_Domain.Entries
{
    public enum GroupTypeEnum
    {
        [Description("Не выбран")]
        None,
        [Description("ВИП")]
        VIP = 1,
        [Description("Куратор")]
        Curator = 2
    }
}
