namespace CatalogServiceSecurityApp.Helpers
{
    using AutoMapper;
    using CatalogServiceSecurityApp.Models.DbModels;
    using CatalogServiceSecurityApp.Models.InputModels;

    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<LoginInputModel, User>().ReverseMap();
            CreateMap<RegisterInputModel, User>().ReverseMap();
            CreateMap<EventInputModel, Event>().ReverseMap();
            CreateMap<UserEventInputModel, UserEvent>().ReverseMap();
        }
    }
}
