using StreamingCourses_Domain.Entries;

namespace StreamingCourses_Contracts.FirebaseModels
{
    public class ShortGroupDataDTO
    {
        public string InviteLink { get; set; }
        public GroupTypeEnum? Type { get; set; }
    }
}
