using AutoMapper;
using prg1.Models;
using prg1.ViewModels;

namespace prg1.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<Category, CategoryModel>().ReverseMap();
            CreateMap<Haber, HaberModel>().ReverseMap();
        }
    }
}