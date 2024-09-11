using NPOI.SS.UserModel;

namespace StreamingCourses_Contracts.Abstractions
{
    public abstract record TemplateBase(byte[] Bytes)
    {
        public abstract Task<ResultBase> Process(List<List<ICell>> cells);
        public virtual T? CreateTypeFromCells<T>(List<ICell> cells) where T : new()
        {
            return Helpers.Helpers.DeserializeObject<T>(cells);
        }
    }
}
