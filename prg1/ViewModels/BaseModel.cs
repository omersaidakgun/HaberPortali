namespace prg1.ViewModels
{
    public class BaseModel
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}